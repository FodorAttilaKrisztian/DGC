using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerController.respawnPoint = transform.position;
        }
    }
}