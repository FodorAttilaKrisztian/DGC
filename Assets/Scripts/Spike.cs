using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Animator animator = collision.GetComponent<Animator>();

        if (damageable != null)
        {
            damageable.health = 0;

            //Notify other subscribed components that this object was hit to handle the knockback and damage
            animator.SetTrigger(AnimationStrings.hitTrigger);
        }
    }
}