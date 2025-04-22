using UnityEngine;

public class MockDamageable : MonoBehaviour
{
    public bool wasHit = false;
    public bool forceHitReturn = false; // Simulate the return value of the Hit() method.

    public bool Hit(int damage, Vector2 knockback)
    {
        wasHit = true;
        return forceHitReturn;
    }
}