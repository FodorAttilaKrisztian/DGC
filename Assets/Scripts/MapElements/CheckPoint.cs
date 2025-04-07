using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && playerController != null)
        {
            // Check if the player is not already at the checkpoint
            if (playerController.respawnPoint.x < transform.position.x)
            {
                // Set the respawn point to this checkpoint's position
                playerController.respawnPoint = transform.position;
                Debug.Log("Checkpoint reached! Respawn point set.");
            }
        }
    }
}