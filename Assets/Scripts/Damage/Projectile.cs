using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private Vector2 moveSpeed = new Vector2(18f, 0f);
    [SerializeField] private Vector2 knockBackForce = new Vector2(5f, 2f);

    [Header("Projectile Type")]
    [SerializeField] private ProjectileType projectileType;
    public enum ProjectileType { Rock, Fireball }

    private Rigidbody2D rb;
    private AudioManager audioManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioManager = AudioManager.instance;
    }

    private void Start()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(moveSpeed.x * direction, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        // Always play hit sound and destroy projectile, even if damage not applied
        PlayImpactSound();

        if (damageable != null)
        {
            Vector2 adjustedKnockback = knockBackForce;
            adjustedKnockback.x *= Mathf.Sign(transform.localScale.x);

            if (damageable.Hit(damage, adjustedKnockback))
            {
                Destroy(gameObject);
                return;
            }
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void PlayImpactSound()
    {
        if (audioManager == null) return;

        switch (projectileType)
        {
            case ProjectileType.Rock:
                audioManager.PlaySFX(audioManager.RockHitSound, 0.2f);
                break;
            case ProjectileType.Fireball:
                audioManager.PlaySFX(audioManager.FireballHitSound, 0.3f);
                break;
        }
    }
}