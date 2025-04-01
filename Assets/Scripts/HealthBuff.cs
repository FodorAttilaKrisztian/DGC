using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/HealthBuff")]
public class HealthBuff : PowerupEffect
{
    public int healAmount;

    public override bool Apply(GameObject target)
    {
        Damageable damageable = target.GetComponent<Damageable>();

        if (damageable)
        {
            bool wasHealed = damageable.Heal(healAmount);
            
            if (wasHealed)
            {
                return true;
            }
        }

        return false;
    }
}