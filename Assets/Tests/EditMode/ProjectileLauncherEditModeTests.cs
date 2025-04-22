using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class ProjectileLauncherEditModeTests
{
    private GameObject launcherGO;
    private ProjectileLauncher projectileLauncher;

    [SetUp]
    public void Setup()
    {
        launcherGO = new GameObject("Launcher");
        projectileLauncher = launcherGO.AddComponent<ProjectileLauncher>();
    }

    [Test]
    public void FirePoint_DefaultsToTransformWhenNotSet()
    {
        Assert.AreEqual(launcherGO.transform, projectileLauncher.FirePoint, "FirePoint should default to the transform if not set.");
    }

    [Test]
    public void ProjectilePrefab_SetAndGet()
    {
        var prefab = new GameObject("ProjectilePrefab");
        projectileLauncher.ProjectilePrefab = prefab;
        Assert.AreEqual(prefab, projectileLauncher.ProjectilePrefab, "ProjectilePrefab getter/setter is not working.");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(launcherGO);
    }
}