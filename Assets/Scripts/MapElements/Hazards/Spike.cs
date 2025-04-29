using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Animator animator = collision.GetComponent<Animator>();

        if (damageable != null && rb != null && damageable.health > 0)
        {
            float horizontalDirection = Mathf.Sign(rb.linearVelocity.x);

            Vector2 deliveredKnockBackForce = new Vector2(horizontalDirection * 2f, 5f);
            
            animator.SetTrigger(AnimationStrings.hitTrigger);

            damageable.Hit(damageable.health, deliveredKnockBackForce);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(deliveredKnockBackForce, ForceMode2D.Impulse);
        }
    }
}