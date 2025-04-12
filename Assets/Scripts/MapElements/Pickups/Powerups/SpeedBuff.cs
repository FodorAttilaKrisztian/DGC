using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerupEffect
{
    public float speedMultiplier;
    public float duration = 8f;

    public override bool Apply(GameObject target)
    {
        PlayerController playerController = target.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.ApplySpeedBuff(speedMultiplier, duration);
            return true;
        }

        return false;
    }
}