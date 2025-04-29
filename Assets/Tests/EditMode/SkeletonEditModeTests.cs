using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkeletonEditModeTests
{
    private GameObject skeletonObj;
    private Skeleton skeleton;
    private Rigidbody2D rb;
    private TouchingDirections touching;

    [SetUp]
    public void Setup()
    {
        skeletonObj = new GameObject("Skeleton");

        rb = skeletonObj.AddComponent<Rigidbody2D>();
        touching = skeletonObj.AddComponent<TouchingDirections>();
        skeleton = skeletonObj.AddComponent<Skeleton>();

        var animator = skeletonObj.GetComponent<Animator>();
        if (animator == null)
        {
            animator = skeletonObj.AddComponent<Animator>();
        }

        skeleton.SetAnimator(animator);
        skeleton.SetRigidbody(rb);
        skeleton.SetTouchingDirections(touching);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(skeletonObj);
    }

    [Test]
    public void WalkDirection_FlipsScaleAndVector()
    {
        Vector2 originalScale = skeleton.transform.localScale;

        skeleton.walkDirection = Skeleton.WalkableDirection.Left;
        Assert.AreEqual(Vector2.left, skeletonObj.GetComponent<Skeleton>().WalkDirectionVector);
        Assert.AreEqual(-originalScale.x, skeleton.transform.localScale.x);
    }

    [Test]
    public void FlipDirection_TogglesWalkDirection()
    {
        skeleton.walkDirection = Skeleton.WalkableDirection.Right;
        skeletonObj.GetComponent<Skeleton>().FlipDirection();
        Assert.AreEqual(Skeleton.WalkableDirection.Left, skeleton.walkDirection);
    }

    [Test]
    public void OnCliffDetected_Flips_WhenGrounded()
    {
        touching.SetGrounded(true);
        skeleton.walkDirection = Skeleton.WalkableDirection.Right;

        skeleton.onCliffDetected();

        Assert.AreEqual(Skeleton.WalkableDirection.Left, skeleton.walkDirection);
    }

    [Test]
    public void OnCliffDetected_DoesNothing_WhenNotGrounded()
    {
        touching.SetGrounded(false);
        skeleton.walkDirection = Skeleton.WalkableDirection.Right;

        skeleton.onCliffDetected();

        Assert.AreEqual(Skeleton.WalkableDirection.Right, skeleton.walkDirection);
    }

    [Test]
    public void OnHit_AppliesKnockback()
    {
        Vector2 knockback = new Vector2(-5, 3);
        rb.linearVelocity = Vector2.zero;

        skeleton.onHit(10, knockback);

        Assert.AreEqual(-5f, rb.linearVelocity.x);
        Assert.AreEqual(3f, rb.linearVelocity.y);
    }

    [Test]
    public void AddToEliminationCounter_UpdatesGameData()
    {
        var gameData = new GameData();
        var mockManager = new GameObject().AddComponent<MockDataPersistenceManager>();
        mockManager.GameData = gameData;
        skeleton.SetDataPersistenceManagerForTesting(mockManager);

        skeleton.AddToEliminationCounter();

        Assert.AreEqual(1, gameData.eliminationsTotal);
        Assert.AreEqual(100, gameData.score);
    }

    [Test]
    public void AttackCooldown_CannotBeNegative()
    {
        skeleton.AttackCooldown = -5f;
        Assert.GreaterOrEqual(skeleton.AttackCooldown, 0f);
    }

    class MockDataPersistenceManager : DataPersistenceManager
    {
        public override void SaveGame() { }
    }
}