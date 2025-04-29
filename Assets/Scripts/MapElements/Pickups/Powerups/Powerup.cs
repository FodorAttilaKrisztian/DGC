using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Powerup : MonoBehaviour, IDataPersistence
{
    [SerializeField] 
    private string persistentID;
    private string id;
    private bool isCollected = false;
    private bool isInitialized = false;

    public PowerupEffect effect;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    public void InitializePersistentID(string persistentId)
    {
        id = persistentId;
    }

    public string GetID()
    {
        return id;
    }

    private void Awake()
    {
        if (!string.IsNullOrEmpty(persistentID))
        {
            id = persistentID;
        }
        else if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }

    private void Start()
    {
        if (!isInitialized && effect != null)
        {
            StartCoroutine(WaitForGameData());
        }
    }

    private void RegisterUncollected()
    {
        GameData data = DataPersistenceManager.instance?.GameData;

        if (data == null)
        {
            Debug.LogError("GameData is null! Powerup cannot be registered.");
            return;
        }

        if (!data.uncollectedPowerups.Exists(p => p.id == id))
        {
            data.uncollectedPowerups.Add(new PowerupData(id, effect.name, transform.position));
        }

        isInitialized = true;
    }

    private IEnumerator WaitForGameData()
    {
        while (DataPersistenceManager.instance == null || DataPersistenceManager.instance.GameData == null)
        {
            yield return null;
        }

        RegisterUncollected();
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

                GameData data = DataPersistenceManager.instance.GameData;
                data.uncollectedPowerups.RemoveAll(p => p.id == id);

                DataPersistenceManager.instance.SaveGame();

                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(AudioManager.instance.PickupSound, 2f);
                }

                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }

    public void LoadData(GameData data){}
    public void SaveData(ref GameData data){}
}