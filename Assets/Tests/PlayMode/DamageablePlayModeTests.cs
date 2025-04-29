using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using System.Reflection;
using UnityEngine.Events;

public class DamageablePlayModeTests
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

        animator.Rebind();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(damageableGO);
    }

    [UnityTest]
    public IEnumerator Hit_ShouldReduceHealth()
    {
        damageable.health = 100;
        damageable.maxHealth = 100;

        damageable.Hit(30, Vector2.zero);

        yield return null;

        Assert.AreEqual(70, damageable.health, "Health should be reduced by 30 after being hit.");
    }

    [UnityTest]
    public IEnumerator Hit_ShouldActivateInvincibility()
    {
        damageable.health = 100;
        damageable.maxHealth = 100;

        damageable.Hit(30, Vector2.zero);

        Assert.IsTrue(damageable.isInvincible, "Damageable should be invincible after being hit.");

        yield return null;

        Assert.IsTrue(damageable.isInvincible, "Damageable should still be invincible within invincibility window.");
    }

    [UnityTest]
    public IEnumerator Hit_ShouldExpireInvincibilityAfterTime()
    {
        damageable.health = 100;
        damageable.maxHealth = 100;
        damageable.isInvincible = true;

        var invincibilityTimeField = typeof(Damageable).GetField("invincibilityTime", BindingFlags.NonPublic | BindingFlags.Instance);
        float invincibilityDuration = (float)invincibilityTimeField.GetValue(damageable);

        damageable.timeSinceHit = invincibilityDuration;

        damageableGO.GetComponent<MonoBehaviour>().Invoke("Update", 0f);

        yield return null;

        Assert.IsFalse(damageable.isInvincible, "Damageable should not be invincible after the invincibility time has passed.");
    }

    [UnityTest]
    public IEnumerator Heal_ShouldIncreaseHealth()
    {
        damageable.health = 50;
        damageable.maxHealth = 100;

        damageable.Heal(30);

        Assert.AreEqual(80, damageable.health, "Health should increase by 30 after healing.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Heal_ShouldNotExceedMaxHealth()
    {
        damageable.health = 90;
        damageable.maxHealth = 100;

        damageable.Heal(30);

        Assert.AreEqual(100, damageable.health, "Health should not exceed max health after healing.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Hit_ShouldNotApplyDamageIfDead()
    {
        damageable.health = 0;
        damageable.maxHealth = 100;
        damageable.isAlive = false;

        bool result = damageable.Hit(30, Vector2.zero);

        yield return null;

        Assert.IsFalse(result, "Damage should not be applied when the damageable is dead.");
    }

    [UnityTest]
    public IEnumerator Hit_ShouldNotApplyDamageIfInvincible()
    {
        damageable.health = 100;
        damageable.maxHealth = 100;

        var invincibilityField = typeof(Damageable).GetField("invincibilityTime", BindingFlags.NonPublic | BindingFlags.Instance);
        invincibilityField.SetValue(damageable, 2f);

        damageable.Hit(30, Vector2.zero);
        int healthAfterFirstHit = damageable.health;

        bool result = damageable.Hit(30, Vector2.zero);
        int healthAfterSecondHit = damageable.health;

        yield return null;

        Assert.AreEqual(70, healthAfterFirstHit, "First hit should reduce health to 70.");
        Assert.IsFalse(result, "Second hit should not apply while invincible.");
        Assert.AreEqual(70, healthAfterSecondHit, "Health should remain the same after second hit due to invincibility.");
    }

    [Test]
    public void Animator_ShouldBeAttached()
    {
        Assert.NotNull(damageableGO.GetComponent<Animator>(), "Animator component should be attached to the GameObject.");
    }

    private void SetPrivateField<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}