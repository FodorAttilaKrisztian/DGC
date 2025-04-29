using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class ProjectileEditModeTests
{
    private GameObject projectileGO;
    private Projectile projectile;

    [SetUp]
    public void Setup()
    {
        projectileGO = new GameObject("Projectile");
        projectile = projectileGO.AddComponent<Projectile>();
        projectileGO.AddComponent<Rigidbody2D>();
    }

    [Test]
    public void Rigidbody2D_ShouldBeAttached()
    {
        Assert.IsNotNull(projectile.GetComponent<Rigidbody2D>(), "Rigidbody2D should be attached to the projectile.");
    }

    [Test]
    public void DefaultMoveSpeed_ShouldBeSet()
    {
        Assert.AreEqual(new Vector2(18f, 0f), projectile.MoveSpeed, "Move speed should match the default.");
    }

    [Test]
    public void DefaultKnockBackForce_ShouldBeSet()
    {
        Assert.AreEqual(new Vector2(5f, 2f), projectile.KnockBackForce, "Knockback force should match the default.");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(projectileGO);
    }
}