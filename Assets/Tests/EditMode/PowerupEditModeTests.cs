using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class PowerupEditModeTests
{
    private GameObject powerupGO;
    private Powerup powerup;
    private GameObject managerGO;

    [SetUp]
    public void SetUp()
    {
        // Set up Powerup GameObject
        powerupGO = new GameObject("TestPowerup");
        powerup = powerupGO.AddComponent<Powerup>();
        powerup.effect = Resources.Load<PowerupEffect>("Powerups/SmallSpeedBuff");

        // Set up DataPersistenceManager
        managerGO = new GameObject("DataPersistenceManager");
        var manager = managerGO.AddComponent<DataPersistenceManager>();
        manager.GameData = new GameData();
        DataPersistenceManager.SetInstanceForTesting(manager);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(powerupGO);
        Object.DestroyImmediate(managerGO);
    }

    [Test]
    public void InitializePersistentID_SetsID()
    {
        string testID = "abc123";
        powerup.InitializePersistentID(testID);
        Assert.AreEqual(testID, powerup.GetID());
    }
}