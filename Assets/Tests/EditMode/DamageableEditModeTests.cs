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
        damageable.health = 100;
        damageable.maxHealth = 100;

        bool result = damageable.Hit(30, Vector2.zero);

        Assert.IsTrue(result);
        Assert.AreEqual(70, damageable.health);
        Assert.IsTrue(damageable.isInvincible);
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
        damageable.health = 0;
        damageable.isAlive = false;

        bool result = damageable.Heal(30);

        Assert.IsFalse(result);

        Assert.AreEqual(0, damageable.health);
        
        Assert.IsFalse(damageable.isAlive);
    }
}