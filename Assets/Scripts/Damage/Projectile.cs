using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(8f, 0);
    public Vector2 knockBackForce = new Vector2(5, 2);

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        float direction = transform.localScale.x != 0 ? transform.localScale.x : 1;
        rb.linearVelocity = new Vector2(moveSpeed.x * direction, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockBackForce = knockBackForce;
            deliveredKnockBackForce.x *= Mathf.Sign(transform.localScale.x);

            bool gotHit = damageable.Hit(damage, deliveredKnockBackForce);

            if (gotHit)
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
}