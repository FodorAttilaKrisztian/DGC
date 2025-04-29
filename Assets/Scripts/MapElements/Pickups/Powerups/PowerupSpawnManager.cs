using UnityEngine;
using System.Collections.Generic;

public class PowerupSpawnManager : MonoBehaviour, IDataPersistence
{
    public static PowerupSpawnManager instance;

    [SerializeField] 
    private List<PowerupEffect> allPowerupTypes;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SaveData(ref GameData data){}

    public void LoadData(GameData data)
    {
        foreach (PowerupData pData in new List<PowerupData>(data.uncollectedPowerups))
        {
            if (!IsPowerupAlreadyInScene(pData.id))
            {
                PowerupEffect effect = allPowerupTypes.Find(e => e.name == pData.effectName);

                if (effect != null && effect.name != "LifeBuff")
                {
                    GameObject prefab = PowerupUtils.LoadPowerupPrefab(pData.effectName);

                    if (prefab != null)
                    {
                        GameObject spawned = Instantiate(prefab, pData.position, Quaternion.identity);
                        Powerup powerup = spawned.GetComponent<Powerup>();
                        
                        if (powerup != null)
                        {
                            powerup.effect = effect;
                            powerup.InitializePersistentID(pData.id);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Powerup prefab not found for: {pData.effectName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"PowerupEffect not found in list for: {pData.effectName}");
                }
            }
        }
    }

    private bool IsPowerupAlreadyInScene(string id)
    {
        foreach (Powerup p in FindObjectsByType<Powerup>(FindObjectsSortMode.None))
        {
            if (p.GetID() == id)
            {
                return true;
            }
        }
        return false;
    }
}