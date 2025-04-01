using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;
    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;

    public int maxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, maxHealth);

            if (_health <= 0)
            {
                isAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    public bool isInvincible = false;

    public float timeSinceHit = 0;

    [SerializeField]
    private float invincibilityTime = 5f;

    public bool isAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("isAlive set: " + value);
        }
    }

    public bool lockVelocity 
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            else
            {
                timeSinceHit += Time.deltaTime;
            }
        }
    }

    public bool Hit(int damage, Vector2 knockBackForce)
    {
        if (isAlive && !isInvincible)
        {
            if (health - damage <= 0)
            {
                health = 0;
            }
            else
            {
                health -= damage;
            }

            isInvincible = true;

            //Notify other subscribed components that this object was hit to handle the knockback and damage
            animator.SetTrigger(AnimationStrings.hitTrigger);
            lockVelocity = true;
            damageableHit?.Invoke(damage, knockBackForce);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }

    public bool Heal(int healAmount)
    {
        if (isAlive && health < maxHealth)
        {
            int maxHeal = Mathf.Max(maxHealth - health, 0);
            int actualHeal = Mathf.Min(maxHeal, healAmount);
            health += actualHeal;
            CharacterEvents.characterHealed(gameObject, actualHeal);
        
            return true;
        }

        return false;
    }
}