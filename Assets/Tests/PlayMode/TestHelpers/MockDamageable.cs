using UnityEngine;

public class MockDamageable : MonoBehaviour
{
    public bool wasHit = false;
    public bool forceHitReturn = false;

    public bool Hit(int damage, Vector2 knockback)
    {
        wasHit = true;
        return forceHitReturn;
    }
}