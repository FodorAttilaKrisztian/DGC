using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Collections;

public class CheckPointEditModeTests
{
    private GameObject checkpointGO;
    private CheckPoint checkpoint;
    private SpriteRenderer spriteRenderer;

    [SetUp]
    public void SetUp()
    {
        checkpointGO = new GameObject("Checkpoint");
        checkpoint = checkpointGO.AddComponent<CheckPoint>();
        spriteRenderer = checkpointGO.AddComponent<SpriteRenderer>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(checkpointGO);
    }

    [Test]
    public void SetToActivated_SetsActivationState()
    {
        checkpoint.SetToActivated();

        Assert.IsTrue(checkpoint.IsActivated());
    }

    [Test]
    public void IsActivated_DefaultsToFalse()
    {
        Assert.IsFalse(checkpoint.IsActivated());
    }
}