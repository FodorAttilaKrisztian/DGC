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
            Debug.Log($"Stored powerup: {powerupType}");
        }
    }

    public void SaveData(ref GameData data)
    {
        data.powerupNames.Clear();

        foreach (var powerupQueue in storedPowerups)
        {
            foreach (var powerup in powerupQueue.Value)
            {
                data.powerupNames.Add(powerup.name);
            }
        }
    }

    public void UsePowerup(string powerupType)
    {
        Debug.Log($"Using powerup i GUESS: {powerupType}");
        
        if (storedPowerups.ContainsKey(powerupType) && storedPowerups[powerupType].Count > 0)
        {
            Debug.Log($"Using powerup: {powerupType}");

            foreach (var item in storedPowerups)
            {
                Debug.Log($"Powerup Type: {item.Key}, Count: {item.Value.Count}");
            }

            if (powerupType == "HealthBuff")
            {
                PowerupEffect powerup = storedPowerups["HealthBuff"].Peek();
                Debug.Log("HealthBuff powerup found in inventory");

                bool applied = powerup.Apply(player);

                if (applied)
                {
                    Debug.Log("HealthBuff applied successfully");
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

    #if UNITY_EDITOR || TEST_MODE
    public void SetPlayer(GameObject player)
    {
        this.player = player;
        var damageable = player.GetComponent<Damageable>();
    
        Debug.Log($"SetPlayer called. Damageable health: {damageable.health}");
    }
    #endif

    #if UNITY_EDITOR || TEST_MODE
    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    #endif
}