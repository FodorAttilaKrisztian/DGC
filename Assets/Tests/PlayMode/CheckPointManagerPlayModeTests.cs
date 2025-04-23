using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class CheckpointManagerPlayModeTests
{
    private GameObject managerGO;
    private CheckpointManager manager;
    private GameObject playerGO;
    private PlayerController player;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        managerGO = new GameObject("CheckpointManager");
        manager = managerGO.AddComponent<CheckpointManager>();

        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        player = playerGO.AddComponent<PlayerController>();

        playerGO.AddComponent<Animator>();

        yield return null; // Let Unity initialize
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(managerGO);
        Object.Destroy(playerGO);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TryActivateCheckpoint_ActivatesNewCheckpoint()
    {
        var checkpointGO = new GameObject("Checkpoint1");
        var checkpoint = checkpointGO.AddComponent<CheckPoint>();
        checkpoint.priority = 1;

        checkpointGO.transform.position = new Vector2(10, 0);

        manager.TryActivateCheckpoint(checkpoint);

        yield return null;

        Assert.IsTrue(checkpoint.IsActivated());
        Assert.AreEqual(new Vector3(10, 0, 0), player.respawnPoint);
        Object.Destroy(checkpointGO);
    }

    [UnityTest]
    public IEnumerator TryActivateCheckpoint_IgnoresLowerPriority()
    {
        // Higher priority one first
        var cp1GO = new GameObject("CP1");
        var cp1 = cp1GO.AddComponent<CheckPoint>();
        cp1.priority = 5;
        manager.TryActivateCheckpoint(cp1);

        // Lower priority one
        var cp2GO = new GameObject("CP2");
        var cp2 = cp2GO.AddComponent<CheckPoint>();
        cp2.priority = 2;

        manager.TryActivateCheckpoint(cp2);

        yield return null;

        Assert.IsTrue(cp1.IsActivated());
        Assert.IsFalse(cp2.IsActivated());

        Object.Destroy(cp1GO);
        Object.Destroy(cp2GO);
    }

    [UnityTest]
    public IEnumerator GetCurrentCheckpointPosition_ReturnsCorrectPosition()
    {
        var checkpointGO = new GameObject("Checkpoint");
        var checkpoint = checkpointGO.AddComponent<CheckPoint>();
        checkpoint.priority = 1;
        checkpointGO.transform.position = new Vector2(42, -10);

        manager.TryActivateCheckpoint(checkpoint);

        yield return null;

        Assert.AreEqual(new Vector2(42, -10), manager.GetCurrentCheckpointPosition());
        Object.Destroy(checkpointGO);
    }

    [UnityTest]
    public IEnumerator GetCurrentCheckpointPosition_ReturnsZeroIfNone()
    {
        Assert.AreEqual(Vector2.zero, manager.GetCurrentCheckpointPosition());
        yield return null;
    }
}