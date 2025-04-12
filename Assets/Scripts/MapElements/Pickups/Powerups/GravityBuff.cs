using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Powerups/GravityBuff")]
public class GravityBuff : PowerupEffect
{
    public float gravityValue = 1.25f;
    public float duration = 6f;

    public override bool Apply(GameObject target)
    {
        PlayerController playerController = target.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.ApplyGravityBuff(gravityValue, duration);
            
            return true;
        }

        return false;
    }

}