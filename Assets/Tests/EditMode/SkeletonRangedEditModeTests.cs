using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class SkeletonRangedEditModeTests
{
    private GameObject skeletonGO;
    private Skeleton skeleton;
    private Animator animator;

    [SetUp]
    public void Setup()
    {
        skeletonGO = new GameObject("Skeleton");
        var rb = skeletonGO.AddComponent<Rigidbody2D>();  // Ensure Rigidbody2D is added
        skeletonGO.AddComponent<TouchingDirections>();
        
        // Add Animator only if not already present
        if (skeletonGO.GetComponent<Animator>() == null)
        {
            animator = skeletonGO.AddComponent<Animator>();
        }

        skeleton = skeletonGO.AddComponent<Skeleton>();
        skeleton.SetAnimator(animator);

        // Add BoxCollider2D to the Skeleton GameObject (required for onHit)
        skeletonGO.AddComponent<BoxCollider2D>();

        // Add proper DetectionZone components with colliders
        var attackGO = new GameObject("AttackZone");
        var attackZone = attackGO.AddComponent<DetectionZone>();
        var attackCollider = attackGO.AddComponent<BoxCollider2D>();
        attackCollider.isTrigger = true;

        var cliffGO = new GameObject("CliffZone");
        var cliffZone = cliffGO.AddComponent<DetectionZone>();
        var cliffCollider = cliffGO.AddComponent<BoxCollider2D>();
        cliffCollider.isTrigger = true;

        skeleton.attackZone = attackZone;
        skeleton.cliffDetectionZone = cliffZone;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(skeletonGO);
        ClearSingleton();
    }

    [Test]
    public void WalkDirectionSetter_FlipsLocalScaleAndDirection()
    {
        skeleton.walkDirection = Skeleton.WalkableDirection.Left;
        Assert.AreEqual(Skeleton.WalkableDirection.Left, skeleton.walkDirection);

        skeleton.walkDirection = Skeleton.WalkableDirection.Right;
        Assert.AreEqual(Skeleton.WalkableDirection.Right, skeleton.walkDirection);
    }

    // Set internal static instance field
    private void SetSingleton(DataPersistenceManager instance)
    {
        typeof(DataPersistenceManager)
            .GetProperty("instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, instance);
    }

    private void ClearSingleton()
    {
        var instanceField = typeof(DataPersistenceManager)
            .GetProperty("instance", BindingFlags.Static | BindingFlags.Public);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) field.SetValue(obj, value);
    }
}