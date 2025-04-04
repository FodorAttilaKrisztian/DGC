using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockBackForce = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockBackForce = transform.parent.localScale.x > 0 ? knockBackForce : new Vector2(-knockBackForce.x, knockBackForce.y);
            
            bool gotHit = damageable.Hit(attackDamage, deliveredKnockBackForce);

            if(gotHit)
            {
                Debug.Log("Hit " + collision.name + " for " + attackDamage + " damage.");
            }
        }
    }
}