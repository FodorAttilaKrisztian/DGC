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
        if (collision.gameObject.name == "Player")
        {
            playerController.respawnPoint = transform.position;
        }
    }
}