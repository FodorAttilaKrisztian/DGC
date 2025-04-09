using UnityEngine;

public class GetFireball : MonoBehaviour, IDataPersistence
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has the tag 'Player'");
            
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerController.instance.hasFireball)
        {
            PlayerController.instance.hasFireball = true;
            UIManager.instance.SetFireballUI(true);  // Use the cached reference
            DataPersistenceManager.instance.SaveGame();

            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.keyPickupSound, 2.5f);
            }

            Destroy(gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        if (data.fireballCollected)
        {
            UIManager.instance.SetFireballUI(true);
            Destroy(gameObject);
        }
        else
        {
            UIManager.instance.SetFireballUI(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.fireballCollected = PlayerController.instance.hasFireball;
    }
}