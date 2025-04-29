using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class PowerupInventoryTests
{
    private GameObject playerObject;
    private PowerupInventory inventory;
    private AudioManager audioManager;
    private PlayerController playerController;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Player");

        playerController = playerObject.AddComponent<PlayerController>();

        var inventoryGO = new GameObject("PowerupInventory");
        inventory = inventoryGO.AddComponent<PowerupInventory>();

        inventory.SetPlayer(playerObject);
        inventory.SetPlayerController(playerController);
    }

    [UnityTest]
    public IEnumerator StorePowerup_AddsToInventory()
    {
        PowerupEffect powerup = Resources.Load<PowerupEffect>("Powerups/SmallSpeedBuff");
        Assert.IsNotNull(powerup, "SmallSpeedBuff not found in Resources/Powerups");

        inventory.StorePowerup(powerup);
        yield return null;

        var stored = inventory.GetStoredPowerups();
        Assert.IsTrue(stored.ContainsKey("SpeedBuff"));
        Assert.AreEqual(1, stored["SpeedBuff"].Count);
    }

    [UnityTest]
    public IEnumerator UsePowerup_RemovesAfterUse()
    {
        PowerupEffect powerup = Resources.Load<PowerupEffect>("Powerups/SmallSpeedBuff");
        Assert.IsNotNull(powerup, "SmallSpeedBuff not found in Resources/Powerups");

        inventory.StorePowerup(powerup);
        yield return null;

        inventory.UsePowerup("SpeedBuff");
        yield return null;

        var stored = inventory.GetStoredPowerups();
        Assert.IsFalse(stored.ContainsKey("SpeedBuff"));
    }
}