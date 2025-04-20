using UnityEngine;

public class KeyPickup : MonoBehaviour, IDataPersistence
{
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has the tag 'Player'");
            
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerController.instance.hasKey)
        {
            PlayerController.instance.hasKey = true;
            UIManager.instance.SetKeyUI(true);  // Use the cached reference
            DataPersistenceManager.instance.SaveGame();

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.KeyPickupSound, 2.5f);
            }

            Destroy(gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        if (data.keyCollected)
        {
            PlayerController.instance.hasKey = true;
            UIManager.instance.SetKeyUI(true);
            Destroy(gameObject);
        }
        else
        {
            PlayerController.instance.hasKey = false;
            UIManager.instance.SetKeyUI(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.keyCollected = PlayerController.instance.hasKey;
    }
}