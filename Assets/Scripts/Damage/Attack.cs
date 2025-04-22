using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 knockBackForce = Vector2.zero;

    public void SetKnockBackForce(Vector2 newKnockbackForce)
    {
        knockBackForce = newKnockbackForce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        var rb = collision.GetComponent<Rigidbody2D>();
        var breakable = collision.GetComponent<Breakable>();

        if (damageable == null)
        {
            return;
        }

        Vector2 deliveredKnockBackForce = Vector2.zero;

        if (breakable == null && rb != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;

            float knockbackX = Mathf.Abs(direction.x) < 0.1f
                ? 0.5f * Mathf.Sign(direction.x != 0 ? direction.x : Random.Range(-1f, 1f))
                : direction.x;

            float knockbackY = Mathf.Abs(direction.y) < 0.1f
                ? 1.5f
                : Mathf.Max(direction.y, 0.5f);

            deliveredKnockBackForce = new Vector2(
                knockbackX * Mathf.Abs(knockBackForce.x),
                knockbackY * Mathf.Abs(knockBackForce.y)
            );

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(deliveredKnockBackForce, ForceMode2D.Impulse);
        }

        damageable.Hit(attackDamage, deliveredKnockBackForce);
    }
}