using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/HealthBuff")]
public class HealthBuff : PowerupEffect
{
    public int healAmount;

    public override bool Apply(GameObject target)
    {
        Damageable damageable = target.GetComponent<Damageable>();

        Debug.Log($"Applying HealthBuff: {healAmount}, Current: {damageable.health}, Max: {damageable.maxHealth}");
        Debug.Log($"[Apply] Player instance: {target.GetInstanceID()}, Health: {target.GetComponent<Damageable>().health}");

        if (damageable)
        {
            bool wasHealed = damageable.Heal(healAmount);

            if (wasHealed)
            {
                Debug.Log($"HealthBuff applied. Heal Amount: {healAmount}, New Health: {damageable.health}");
                return true;
            }
            else
            {
                Debug.Log("Failed to heal player.");
            }
        }
        else
        {
            Debug.Log("No Damageable component found on target.");
        }

        return false;
    }
}