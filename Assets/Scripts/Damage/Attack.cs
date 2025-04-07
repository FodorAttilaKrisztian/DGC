using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockBackForce = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Breakable breakable = collision.GetComponent<Breakable>();

        if ((damageable != null && rb != null) || (damageable != null && breakable != null))
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;

            float minX = 0.5f * Mathf.Sign(direction.x != 0 ? direction.x : Random.Range(-1f, 1f));
            float knockbackX = Mathf.Abs(direction.x) < 0.1f ? minX : direction.x;

            float minY = 1.5f; // force strong upward launch
            float knockbackY = Mathf.Abs(direction.y) < 0.1f ? minY : Mathf.Max(direction.y, 0.5f);

            Vector2 deliveredKnockBackForce = new Vector2(knockbackX * Mathf.Abs(knockBackForce.x), knockbackY * Mathf.Abs(knockBackForce.y));

            if ((damageable != null && rb != null))
            {
                // Apply knockback directly to Rigidbody2D
                rb.linearVelocity = Vector2.zero; // reset current velocity to prevent downward drag
                rb.AddForce(deliveredKnockBackForce, ForceMode2D.Impulse);   
            }

            damageable.Hit(attackDamage, deliveredKnockBackForce);
        }
    }
}