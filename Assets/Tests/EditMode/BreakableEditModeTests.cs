using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class BreakableEditModeTests
{
    private GameObject breakableGO;
    private Breakable breakable;

    [SetUp]
    public void SetUp()
    {
        breakableGO = new GameObject("TestBreakable");
        breakable = breakableGO.AddComponent<Breakable>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(breakableGO);
    }

    [Test]
    public void GenerateGuid_GeneratesNewID()
    {
        // Generate a GUID for the breakable object
        breakable.GenerateGuid();

        string id = breakable.GetID();
        Assert.IsFalse(string.IsNullOrEmpty(id), "ID should not be empty.");
    }
}