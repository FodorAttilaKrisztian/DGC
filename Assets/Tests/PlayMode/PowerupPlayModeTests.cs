using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PowerupPlayModeTests
{
    private GameObject powerupGO;
    private Powerup powerup;
    private GameObject dpManagerGO;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        dpManagerGO = new GameObject("DataPersistenceManager");
        var testManager = dpManagerGO.AddComponent<TestDataPersistenceManager>();
        testManager.Awake();
        testManager.GameData = new GameData();

        yield return null;

        powerupGO = new GameObject("TestPowerup");
        powerup = powerupGO.AddComponent<Powerup>();
        powerup.effect = Resources.Load<PowerupEffect>("Powerups/SmallSpeedBuff");
        powerup.InitializePersistentID("powerup-test-id");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Start_RegistersUncollectedPowerup()
    {
        yield return new WaitForSeconds(0.1f);

        var data = DataPersistenceManager.instance.GameData;
        var registered = data.uncollectedPowerups.Find(p => p.id == "powerup-test-id");

        Assert.IsNotNull(registered);
        Assert.AreEqual("SmallSpeedBuff", registered.effectName);
    }

    [UnityTest]
    public IEnumerator Awake_GeneratesGUID_IfIDEmpty()
    {
        yield return null;

        string id = powerup.GetID();
        Assert.IsFalse(string.IsNullOrEmpty(id));
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(powerupGO);
        Object.Destroy(dpManagerGO);
        yield return null;
    }
}

public class TestDataPersistenceManager : DataPersistenceManager
{
    public void Awake()
    {
        DataPersistenceManager.SetInstanceForTesting(this);
        GameData = new GameData();
    }

    public void Start()
    {}
}