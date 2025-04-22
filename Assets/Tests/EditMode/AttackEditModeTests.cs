using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[TestFixture]
public class AttackEditModeTests
{
    private GameObject attackObject;
    private Attack attackComponent;

    [SetUp]
    public void SetUp()
    {
        attackObject = new GameObject("AttackObject");
        attackComponent = attackObject.AddComponent<Attack>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(attackObject);
    }

    [Test]
    public void Attack_ShouldHaveCorrectInitialValues()
    {
        // Checking initial values set by default in the inspector
        var attack = attackComponent;

        // Assert default attack damage
        Assert.AreEqual(10, attack.GetType().GetField("attackDamage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(attack));

        // Assert default knockback force (should be zero initially)
        var knockBackForce = (Vector2)attack.GetType().GetField("knockBackForce", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(attack);
        Assert.AreEqual(Vector2.zero, knockBackForce);
    }
}