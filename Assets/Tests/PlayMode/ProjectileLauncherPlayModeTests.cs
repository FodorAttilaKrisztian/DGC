using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;

public class ProjectileLauncherPlayModeTests
{
    private GameObject launcherGO;
    private ProjectileLauncher projectileLauncher;
    private GameObject firePointGO;
    private GameObject projectilePrefab;

    [SetUp]
    public void Setup()
    {
        launcherGO = new GameObject("Launcher");
        projectileLauncher = launcherGO.AddComponent<ProjectileLauncher>();

        firePointGO = new GameObject("FirePoint");
        firePointGO.transform.position = new Vector3(2, 0, 0);
        launcherGO.transform.SetParent(firePointGO.transform);

        typeof(ProjectileLauncher)
            .GetField("firePoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(projectileLauncher, firePointGO.transform);

        projectilePrefab = new GameObject("ProjectilePrefab");
        projectilePrefab.AddComponent<Projectile>();

        if (!projectilePrefab.TryGetComponent<Rigidbody2D>(out _))
        {
            var rb = projectilePrefab.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        projectilePrefab.AddComponent<BoxCollider2D>();

        projectileLauncher.ProjectilePrefab = projectilePrefab;
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(launcherGO);
        Object.Destroy(firePointGO);
        Object.Destroy(projectilePrefab);

        foreach (var p in GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None))
        {
            Object.Destroy(p.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator FireProjectile_InstantiatesProjectile()
    {
        projectileLauncher.FireProjectile();
        yield return null;

        var projectiles = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);

        Assert.IsNotEmpty(projectiles, "Expected a projectile to be instantiated");
    }

    [UnityTest]
    public IEnumerator FireProjectile_FlipsProjectileIfImplemented()
    {
        launcherGO.transform.localScale = new Vector3(-1, 1, 1);

        projectileLauncher.FireProjectile();
        yield return null;

        var projectiles = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);
        Assert.IsNotEmpty(projectiles, "Expected a projectile to be instantiated");

        var projectile = projectiles[0];

        Debug.Log($"Projectile localScale.x: {projectile.transform.localScale.x}");
    }

    [UnityTest]
    public IEnumerator FireProjectile_LogsWarningIfPrefabOrFirePointIsMissing()
    {
        var before = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);

        projectileLauncher.ProjectilePrefab = null;
        typeof(ProjectileLauncher)
            .GetField("firePoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(projectileLauncher, null);

        var logHandler = new LogHandler();
        Application.logMessageReceived += logHandler.LogMessageReceived;

        projectileLauncher.FireProjectile();
        yield return null;

        var after = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);

        Assert.AreEqual(before.Length, after.Length, "No projectile should be instantiated if prefab or firepoint is missing.");
        Assert.IsTrue(logHandler.ContainsWarning("Missing firePoint or projectilePrefab"), "Expected warning not found in logs.");

        Application.logMessageReceived -= logHandler.LogMessageReceived;
    }
}

public class LogHandler
{
    private string logMessages = "";

    public void LogMessageReceived(string logString, string stackTrace, LogType type)
    {
        logMessages += logString + "\n";
    }

    public bool ContainsWarning(string warningMessage)
    {
        return logMessages.Contains(warningMessage);
    }
}