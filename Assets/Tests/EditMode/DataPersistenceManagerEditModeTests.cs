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
        GameObject gameObject = new GameObject();
        dataPersistenceManager = gameObject.AddComponent<DataPersistenceManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(dataPersistenceManager.gameObject);
    }

    [Test]
    public void NewGame_InitializesWithDefaultData()
    {
        dataPersistenceManager.NewGame();

        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP);
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount);
    }
}