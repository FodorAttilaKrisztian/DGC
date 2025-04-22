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
        // Create the damageable GameObject and add the necessary components
        damageableGO = new GameObject("Damageable");
        damageable = damageableGO.AddComponent<Damageable>();

        // Add the Animator and set up the RuntimeAnimatorController
        var animator = damageableGO.AddComponent<Animator>();
        var controllerPath = "Assets/Tests/Animation/AnimatorControllers/TestAnimatorController.controller";
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        animator.runtimeAnimatorController = controller;

        // Reset the Animator state before testing (just making sure it's ready)
        animator.Rebind();  // Reset Animator (non-animation test case)
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up by destroying the GameObject
        Object.DestroyImmediate(damageableGO);
    }

    // Test: Verify that damage reduces health
    [UnityTest]
    public IEnumerator Hit_ShouldReduceHealth()
    {
        // Arrange: Set up initial health values
        damageable.health = 100;
        damageable.maxHealth = 100;

        // Act: Simulate a hit event
        damageable.Hit(30, Vector2.zero);

        // Wait for a frame to simulate damage processing
        yield return null;

        // Assert: Check that health is reduced correctly
        Assert.AreEqual(70, damageable.health, "Health should be reduced by 30 after being hit.");
    }

    // Test: Verify invincibility behavior
    [UnityTest]
    public IEnumerator Hit_ShouldActivateInvincibility()
    {
        // Arrange: Set up initial health values
        damageable.health = 100;
        damageable.maxHealth = 100;

        // Act: Simulate a hit event
        damageable.Hit(30, Vector2.zero);

        // Assert: Check if invincibility is activated after the first hit
        Assert.IsTrue(damageable.isInvincible, "Damageable should be invincible after being hit.");

        // Wait for a frame and check again
        yield return null;

        // Assert: Still invincible immediately after being hit
        Assert.IsTrue(damageable.isInvincible, "Damageable should still be invincible within invincibility window.");
    }

    [UnityTest]
    public IEnumerator Hit_ShouldExpireInvincibilityAfterTime()
    {
        // Arrange: Set up initial health values and invincibility duration
        damageable.health = 100;
        damageable.maxHealth = 100;
        damageable.isInvincible = true;  // Manually activate invincibility for testing

        // Use reflection to read invincibilityTime value
        var invincibilityTimeField = typeof(Damageable).GetField("invincibilityTime", BindingFlags.NonPublic | BindingFlags.Instance);
        float invincibilityDuration = (float)invincibilityTimeField.GetValue(damageable);

        // Simulate that invincibility has expired
        damageable.timeSinceHit = invincibilityDuration;

        // Call Update to let it process the expiration logic
        damageableGO.GetComponent<MonoBehaviour>().Invoke("Update", 0f); // Not ideal, but helps simulate Update manually

        yield return null;

        // Assert: Check that invincibility is deactivated
        Assert.IsFalse(damageable.isInvincible, "Damageable should not be invincible after the invincibility time has passed.");
    }

    [UnityTest]
    public IEnumerator Heal_ShouldIncreaseHealth()
    {
        // Arrange: Set up initial health values
        damageable.health = 50;
        damageable.maxHealth = 100;

        // Act: Heal the damageable by 30
        damageable.Heal(30);

        // Assert: Check that health is increased correctly
        Assert.AreEqual(80, damageable.health, "Health should increase by 30 after healing.");

        // Yield so Unity doesn't complain
        yield return null;
    }

    [UnityTest]
    public IEnumerator Heal_ShouldNotExceedMaxHealth()
    {
        // Arrange: Set up initial health values
        damageable.health = 90;
        damageable.maxHealth = 100;

        // Act: Heal the damageable by 30, which would exceed max health
        damageable.Heal(30);

        // Assert: Check that health does not exceed max health
        Assert.AreEqual(100, damageable.health, "Health should not exceed max health after healing.");

        // Yield so Unity doesn't complain
        yield return null;
    }

    // Test: Verify that dead damageables cannot take damage
    // Test: Verify that damageable cannot take damage if dead
    [UnityTest]
    public IEnumerator Hit_ShouldNotApplyDamageIfDead()
    {
        // Arrange: Set up initial health values and set the damageable as dead
        damageable.health = 0;
        damageable.maxHealth = 100;
        damageable.isAlive = false;

        // Act: Try to apply damage while dead
        bool result = damageable.Hit(30, Vector2.zero);

        // Wait a frame to simulate damage application
        yield return null;

        // Assert: Check that damage is not applied when the damageable is dead
        Assert.IsFalse(result, "Damage should not be applied when the damageable is dead.");
    }

    [UnityTest]
    public IEnumerator Hit_ShouldNotApplyDamageIfInvincible()
    {
        // Arrange
        damageable.health = 100;
        damageable.maxHealth = 100;

        // Use reflection to shorten invincibility time (optional, but useful for fast tests)
        var invincibilityField = typeof(Damageable).GetField("invincibilityTime", BindingFlags.NonPublic | BindingFlags.Instance);
        invincibilityField.SetValue(damageable, 2f);

        // Act: First hit – should trigger invincibility
        damageable.Hit(30, Vector2.zero);
        int healthAfterFirstHit = damageable.health;

        // Act: Immediately try another hit during invincibility
        bool result = damageable.Hit(30, Vector2.zero);
        int healthAfterSecondHit = damageable.health;

        yield return null;

        // Assert
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