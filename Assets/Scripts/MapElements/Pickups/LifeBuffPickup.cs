using UnityEngine;

public class LifeBuffPickup : MonoBehaviour, IDataPersistence
{
    [SerializeField] private LifeBuff lifeBuff;
    [SerializeField] private string id;

    private bool hasBeenPickedUp = false;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();  // Assign a new GUID if missing
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hasBeenPickedUp && lifeBuff.Apply(collision.gameObject))
            {
                hasBeenPickedUp = true;

                DataPersistenceManager.instance.SaveGame();

                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(AudioManager.instance.KeyPickupSound, 2.5f);
                }

                Destroy(gameObject);
            }
        }
    }

    public void LoadData(GameData data)
    {
        if (data.livesCollected.TryGetValue(id, out hasBeenPickedUp) && hasBeenPickedUp)
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.livesCollected.ContainsKey(id))
        {
            data.livesCollected.Remove(id);
        }

        data.livesCollected.Add(id, hasBeenPickedUp);
    }
}