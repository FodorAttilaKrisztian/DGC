using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField]
    private int _health = 100;

    [Header("Invincibility Settings")]
    [SerializeField]
    private float invincibilityTime = 5f;
    [SerializeField]
    public bool isInvincible = false;

    [Header("Status Flags")]
    [SerializeField] private bool _isAlive = true;

    [Header("Events")]
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;

    public float timeSinceHit = 0f;
    private Animator animator;

    // --- Properties ---
    public int maxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public int health
    {
        get => _health;
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, maxHealth);
            if (_health <= 0 && isAlive) isAlive = false;
        }
    }

    public bool isAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;

            if (animator != null)
            {
                animator.SetBool(AnimationStrings.isAlive, value);
            }
        }
    }

    public bool lockVelocity
    {
        get => animator.GetBool(AnimationStrings.lockVelocity);
        set => animator.SetBool(AnimationStrings.lockVelocity, value);
    }

    // --- Unity Methods ---
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isInvincible) return;

        timeSinceHit += Time.deltaTime;

        if (timeSinceHit >= invincibilityTime)
        {
            isInvincible = false;
            timeSinceHit = 0f;
        }
    }

    // --- Public Methods ---
    public virtual bool Hit(int damage, Vector2 knockBackForce)
    {
        if (!isAlive || isInvincible) return false;

        health = Mathf.Max(health - damage, 0);
        isInvincible = true;

        if (animator != null)
        {
            animator.SetTrigger(AnimationStrings.hitTrigger);
            lockVelocity = true;
        }

        damageableHit?.Invoke(damage, knockBackForce);
        CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

        return true;
    }

    public bool Heal(int healAmount)
    {
        if (!isAlive || health >= maxHealth) return false;

        int actualHeal = Mathf.Min(maxHealth - health, healAmount);
        health += actualHeal;

        CharacterEvents.characterHealed?.Invoke(gameObject, actualHeal);
        return true;
    }
}