using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/LifeBuff")]
public class LifeBuff : PowerupEffect
{
    public override bool Apply(GameObject target)
    {
        PlayerController playerController = target.GetComponent<PlayerController>();

        if (playerController != null)
        {
            if (playerController.currentLives < playerController.maxLives)
            {
                playerController.currentLives++;

                return true;
            }
        }

        return false;
    }
}