using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class PowerupInventory : MonoBehaviour
{
    private Dictionary<string, Queue<PowerupEffect>> storedPowerups = new Dictionary<string, Queue<PowerupEffect>>();
    public UnityEvent PowerupChanged;

    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.actions["UseHealthBuff"].performed += _ => UsePowerup("HealthBuff");
            playerInput.actions["UseSpeedBuff"].performed += _ => UsePowerup("SpeedBuff");
            playerInput.actions["UseGravityBuff"].performed += _ => UsePowerup("GravityBuff");
        }

        if (PowerupChanged == null)
        {
            PowerupChanged = new UnityEvent();
        }
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
}