using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class DataPersistenceManagerEditModeTests
{
    private DataPersistenceManager dataPersistenceManager;

    [SetUp]
    public void Setup()
    {
        // Create a new DataPersistenceManager in EditMode.
        GameObject gameObject = new GameObject();
        dataPersistenceManager = gameObject.AddComponent<DataPersistenceManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(dataPersistenceManager.gameObject);  // Clean up after each test
    }

    [Test]
    public void NewGame_InitializesWithDefaultData()
    {
        // Test that a new game initializes with default data.
        dataPersistenceManager.NewGame();

        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP);
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount);
    }
}