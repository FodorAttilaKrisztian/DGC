using UnityEngine;

public class Powerup : MonoBehaviour, IDataPersistence
{
    private string id;
    private bool isCollected = false;
    private bool isInitialized = false;

    public PowerupEffect effect;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    public void InitializePersistentID(string persistentId)
    {
        id = persistentId;

        SetupFromEffect();
    }

    public string GetID()
    {
        return id;
    }

    private void Awake()
    {
        // Generate an ID if this is a brand-new (non-persistent) powerup
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }

        // Delay adding to GameData until we're fully initialized
    }

    private void Start()
    {
        // At this point, effect should be assigned (by spawn manager)
        // Now we can safely register to GameData
        if (!isInitialized && effect != null)
        {
            RegisterUncollected();
        }
    }

    private void RegisterUncollected()
    {
        GameData data = DataPersistenceManager.instance.GameData;

        if (!data.uncollectedPowerups.Exists(p => p.id == id))
        {
            data.uncollectedPowerups.Add(new PowerupData(id, effect.name, transform.position));
        }

        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            PowerupInventory inventory = FindFirstObjectByType<PowerupInventory>();

            if (inventory != null && effect.name != "LifeBuff")
            {
                inventory.StorePowerup(effect);
                isCollected = true;

                // Remove from GameData (collected)
                GameData data = DataPersistenceManager.instance.GameData;
                data.uncollectedPowerups.RemoveAll(p => p.id == id);

                DataPersistenceManager.instance.SaveGame();
                Destroy(gameObject);
            }
        }
    }

    private void SetupFromEffect()
    {
        // Add visual or audio setup based on effect here
        // Safe to do now since effect is assigned in the spawner
    }

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }

    public void LoadData(GameData data){}
    public void SaveData(ref GameData data){}
}