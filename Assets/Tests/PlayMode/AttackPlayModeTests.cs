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
        attackObject = new GameObject("AttackObject");
        damageableObject = new GameObject("DamageableObject");

        attackObject.AddComponent<BoxCollider2D>().isTrigger = true;
        attackObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        attackObject.AddComponent<Attack>();

        damageableObject.AddComponent<BoxCollider2D>();
        damageableRb = damageableObject.AddComponent<Rigidbody2D>();
        damageableAnimator = damageableObject.AddComponent<Animator>();
        damageableObject.AddComponent<Damageable>();

        var path = "Assets/Tests/Animation/AnimatorControllers/TestAnimatorController.controller";
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
        damageableAnimator.runtimeAnimatorController = controller;

        Assert.IsNotNull(controller, "Missing TestAnimatorController in Resources folder!");

        var damageableScript = damageableObject.GetComponent<Damageable>();
        var animatorField = typeof(Damageable).GetField("animator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(animatorField, "Animator field not found via reflection!");

        animatorField.SetValue(damageableScript, damageableAnimator);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(attackObject);
        Object.DestroyImmediate(damageableObject);

        CharacterEvents.characterDamaged = null;
        CharacterEvents.characterHealed = null;
    }

    [UnityTest]
    public IEnumerator Attack_ShouldTriggerHitOnDamageable()
    {
        CharacterEvents.characterDamaged += (_, _) => { };
        CharacterEvents.characterHealed += (_, _) => { };

        attackObject.transform.position = Vector2.zero;
        damageableObject.transform.position = Vector2.zero;

        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(0.1f);

        var rb = damageableObject.GetComponent<Rigidbody2D>();
        Assert.IsTrue(rb.linearVelocity.magnitude > 0 || rb.IsSleeping() == false, "Expected knockback to be applied.");
    }
}