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
        // Create and initialize DataPersistenceManager
        dpManagerGO = new GameObject("DataPersistenceManager");
        var testManager = dpManagerGO.AddComponent<TestDataPersistenceManager>();
        testManager.Awake(); // Initialize
        testManager.GameData = new GameData();

        yield return null; // Wait a frame so manager becomes "active"

        // Now set up the powerup
        powerupGO = new GameObject("TestPowerup");
        powerup = powerupGO.AddComponent<Powerup>();
        powerup.effect = Resources.Load<PowerupEffect>("Powerups/SmallSpeedBuff");
        powerup.InitializePersistentID("powerup-test-id");

        yield return null; // Let Start() on Powerup run
    }

    [UnityTest]
    public IEnumerator Start_RegistersUncollectedPowerup()
    {
        yield return new WaitForSeconds(0.1f); // Let coroutine complete

        var data = DataPersistenceManager.instance.GameData;
        var registered = data.uncollectedPowerups.Find(p => p.id == "powerup-test-id");

        Assert.IsNotNull(registered);
        Assert.AreEqual("SmallSpeedBuff", registered.effectName);
    }

    [UnityTest]
    public IEnumerator Awake_GeneratesGUID_IfIDEmpty()
    {
        // We don’t need to manually call Awake() here as Unity will handle it
        yield return null; // Let Unity run one frame

        // Now check that the GUID is generated
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
    {
        // Skip base Start() to avoid auto-loading/saving
    }
}