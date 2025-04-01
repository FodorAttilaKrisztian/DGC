using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerTrigger"))
        {
            playerController.isNearExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerTrigger"))
        {
            playerController.isNearExit = false;
        }
    }
}