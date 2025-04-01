using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject keyUI;
    PlayerController playerController;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has the tag 'Player'");
            return;
        }

        playerController = player.GetComponent<PlayerController>();

        if (keyUI != null)
        {
            keyUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerController.hasKey)
            {
                keyUI.SetActive(true);
                playerController.hasKey = true;
                Destroy(gameObject);
            }
        }
    }
}