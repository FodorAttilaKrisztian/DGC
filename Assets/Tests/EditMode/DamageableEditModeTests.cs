using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using System.Reflection;
using UnityEngine.Events;

public class DamageableEditModeTests
{
    private GameObject damageableGO;
    private Damageable damageable;

    [SetUp]
    public void SetUp()
    {
        damageableGO = new GameObject("Damageable");
        damageable = damageableGO.AddComponent<Damageable>();

        var animator = damageableGO.AddComponent<Animator>();
        var controllerPath = "Assets/Tests/Animation/AnimatorControllers/TestAnimatorController.controller";
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        animator.runtimeAnimatorController = controller;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(damageableGO);
    }

    [Test]
    public void Hit_ShouldReduceHealth_WhenAliveAndNotInvincible()
    {
        // Arrange: Set health to 100 and maxHealth to 100, and ensure animator is set
        damageable.health = 100;
        damageable.maxHealth = 100;

        // Act: Call the Hit method with damage value of 30
        bool result = damageable.Hit(30, Vector2.zero);

        // Assert: Ensure the result is true, health is reduced and invincibility is triggered
        Assert.IsTrue(result);
        Assert.AreEqual(70, damageable.health);
        Assert.IsTrue(damageable.isInvincible);

        // Assert: Ensure that the animator trigger was set (you can use a mock or check the internal state)
        // Note: If testing the animator directly, you can use a mock or check if animator.SetTrigger was called.
    }

    [Test]
    public void Hit_ShouldReturnFalse_WhenDead()
    {
        damageable.health = 0;
        damageable.isAlive = false;

        bool result = damageable.Hit(10, Vector2.zero);

        Assert.IsFalse(result);
        Assert.AreEqual(0, damageable.health);
    }

    [Test]
    public void Heal_ShouldIncreaseHealth_UpToMax()
    {
        damageable.health = 50;
        damageable.maxHealth = 100;

        bool result = damageable.Heal(30);

        Assert.IsTrue(result);
        Assert.AreEqual(80, damageable.health);
    }

    [Test]
    public void Heal_ShouldNotIncreaseHealth_IfDead()
    {
        // Arrange: Set health to 0 and isAlive to false
        damageable.health = 0;
        damageable.isAlive = false;

        // Act: Try to heal even though the object is dead
        bool result = damageable.Heal(30);

        // Assert: Result should be false, meaning no healing is possible
        Assert.IsFalse(result);

        // Assert: Health should remain 0 (no increase if dead)
        Assert.AreEqual(0, damageable.health);
        
        // Ensure animator state is not triggered when dead (if relevant)
        Assert.IsFalse(damageable.isAlive); // Ensure isAlive hasn't been set to true
    }
}