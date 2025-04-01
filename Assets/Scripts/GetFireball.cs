using UnityEngine;

public class GetFireball : MonoBehaviour
{
    public GameObject fireballUI;
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

        if (fireballUI != null)
        {
            fireballUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerController.hasFireball)
            {
                fireballUI.SetActive(true);
                playerController.hasFireball = true;
                Destroy(gameObject);
            }
        }
    }
}