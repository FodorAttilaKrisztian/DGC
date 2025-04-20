using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class PowerupInventory : MonoBehaviour, IDataPersistence
{
    private Dictionary<string, Queue<PowerupEffect>> storedPowerups = new Dictionary<string, Queue<PowerupEffect>>();
    public UnityEvent PowerupChanged;
    private AudioManager audioManager;
    private PlayerController playerController;
    private GameObject player;

    private void Awake()
    {
        audioManager = AudioManager.instance;

        if (audioManager == null)
        {
            Debug.LogError("AudioManager instance not found in the scene.");
        }
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for PlayerController.instance to be assigned
        while (PlayerController.instance == null)
            yield return null;

        playerController = PlayerController.instance;
        
        Debug.Log("PlayerController instance found: " + playerController.name);

        player = playerController.gameObject;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
            yield break;
        }

        PowerupChanged?.Invoke();
    }

    public void StorePowerup(PowerupEffect powerup)
    {
        string powerupType = GetPowerupType(powerup);

        if (powerupType != "LifeBuff" && powerupType != "FireballBuff")
        {
            if (!storedPowerups.ContainsKey(powerupType))
            {
                storedPowerups[powerupType] = new Queue<PowerupEffect>();
            }

            storedPowerups[powerupType].Enqueue(powerup);

            PowerupChanged?.Invoke();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.powerupNames.Clear();

        foreach (var powerupQueue in storedPowerups)
        {
            foreach (var powerup in powerupQueue.Value)
            {
                data.powerupNames.Add(powerup.name); // Save the ScriptableObject name
            }
        }
    }

    public void UsePowerup(string powerupType)
    {
        if (storedPowerups.ContainsKey(powerupType) && storedPowerups[powerupType].Count > 0)
        {
            if (powerupType == "HealthBuff")
            {
                PowerupEffect powerup = storedPowerups["HealthBuff"].Peek();
                
                bool applied = powerup.Apply(player);

                if (applied)
                {
                    storedPowerups["HealthBuff"].Dequeue();

                    if (storedPowerups[powerupType].Count == 0)
                    {
                        storedPowerups.Remove(powerupType);
                    }
                    
                    PowerupChanged?.Invoke();

                    PlayHealSound();
                }
            }
            else
            {
                PowerupEffect powerup = storedPowerups[powerupType].Dequeue();
                powerup.Apply(player);

                if (storedPowerups[powerupType].Count == 0)
                {
                    storedPowerups.Remove(powerupType);
                }

                PowerupChanged?.Invoke();

                if (powerupType == "SpeedBuff")
                {
                    PlaySpeedSound();
                }
                else if (powerupType == "GravityBuff")
                {
                    PlayGravitySound();
                }
            }
        }
    }

    public void LoadData(GameData data)
    {
        storedPowerups.Clear();
        
        foreach (string powerupName in data.powerupNames)
        {
            PowerupEffect loadedPowerup = Resources.Load<PowerupEffect>($"Powerups/{powerupName}");

            if (loadedPowerup != null)
            {
                StorePowerup(loadedPowerup);
            }
            else
            {
                Debug.LogError($"Failed to load powerup: {powerupName}");
            }
        }
    }

    public Dictionary<string, Queue<PowerupEffect>> GetStoredPowerups()
    {
        return storedPowerups;
    }

    private string GetPowerupType(PowerupEffect powerup)
    {
        if (powerup.name.Contains("Speed")) return "SpeedBuff";
        if (powerup.name.Contains("Gravity")) return "GravityBuff";
        if (powerup.name.Contains("Health")) return "HealthBuff";
        if (powerup.name.Contains("Fireball")) return "FireballBuff";
        if (powerup.name.Contains("Life")) return "LifeBuff";

        return "";
    }

    public void PlayHealSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.HealSound, 0.25f);
        }
    }

    public void PlaySpeedSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.SpeedSound, 0.25f);
        }
    }

    public void PlayGravitySound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.GravitySound, 0.25f);
        }
    }
}