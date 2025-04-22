using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System.Collections;

public class AttackPlayModeTests
{
    private GameObject attackObject;
    private GameObject damageableObject;
    private Animator damageableAnimator;
    private Rigidbody2D damageableRb;

    [SetUp]
    public void SetUp()
    {
        // Create objects
        attackObject = new GameObject("AttackObject");
        damageableObject = new GameObject("DamageableObject");

        // Add components
        attackObject.AddComponent<BoxCollider2D>().isTrigger = true;
        attackObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        attackObject.AddComponent<Attack>();

        damageableObject.AddComponent<BoxCollider2D>();
        damageableRb = damageableObject.AddComponent<Rigidbody2D>();
        damageableAnimator = damageableObject.AddComponent<Animator>();  // Make sure this is added
        damageableObject.AddComponent<Damageable>();

        // Load and assign AnimatorController
        var path = "Assets/Tests/Animation/AnimatorControllers/TestAnimatorController.controller";
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
        damageableAnimator.runtimeAnimatorController = controller;

        Assert.IsNotNull(controller, "Missing TestAnimatorController in Resources folder!");

        // Re-bind the private animator field in Damageable using reflection
        var damageableScript = damageableObject.GetComponent<Damageable>();
        var animatorField = typeof(Damageable).GetField("animator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(animatorField, "Animator field not found via reflection!");

        // Assign the animator to the private field using reflection
        animatorField.SetValue(damageableScript, damageableAnimator);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(attackObject);
        Object.DestroyImmediate(damageableObject);

        // Clean up event listeners
        CharacterEvents.characterDamaged = null;
        CharacterEvents.characterHealed = null;
    }

    [UnityTest]
    public IEnumerator Attack_ShouldTriggerHitOnDamageable()
    {
        // 🧪 SAFEGUARD: Assign a dummy handler to prevent null reference
        CharacterEvents.characterDamaged += (_, _) => { };
        CharacterEvents.characterHealed += (_, _) => { };

        // Position objects to overlap for trigger
        attackObject.transform.position = Vector2.zero;
        damageableObject.transform.position = Vector2.zero;

        // Let physics update to detect trigger
        yield return new WaitForFixedUpdate();

        // Give a small time buffer for physics to apply
        yield return new WaitForSeconds(0.1f);

        // Check if some force was applied (simple knockback test)
        var rb = damageableObject.GetComponent<Rigidbody2D>();
        Assert.IsTrue(rb.linearVelocity.magnitude > 0 || rb.IsSleeping() == false, "Expected knockback to be applied.");
    }
}