using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

public class PlayerControllerEditModeTests
{
    private GameObject playerObj;
    private PlayerController playerController;
    private Damageable damageable;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private PowerupInventory powerupInventory;

    [SetUp]
    public void Setup()
    {
        playerObj = new GameObject("Player");
        
        playerController = playerObj.AddComponent<PlayerController>();
        damageable = playerObj.AddComponent<Damageable>();
        playerController.maxLives = 3;
        rb = playerObj.GetComponent<Rigidbody2D>() ?? playerObj.AddComponent<Rigidbody2D>();
        touchingDirections = playerObj.AddComponent<TouchingDirections>();
        powerupInventory = playerObj.AddComponent<PowerupInventory>();
        powerupInventory.SetPlayer(playerObj);

        playerController.SetRigidbody(rb);
        playerController.SetDamageable(damageable);
        playerController.SetTouchingDirections(touchingDirections);
        playerController.SetPowerupInventory(powerupInventory);
    
        playerController.playerDamageable.maxHealth = 100;
        
        playerController.playerDamageable.health = 60;
        Debug.Log($"Initial health: {playerController.playerDamageable.health}");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void PlayerStartsWithCorrectLives()
    {
        Assert.AreEqual(playerController.currentLives, playerController.maxLives);
    }

    [Test]
    public void PlayerCanUseSpeedBuff()
    {
        playerController.ApplySpeedBuff(2f, 5f);
        Assert.AreEqual(playerController.walkSpeed, 6f);
        Assert.AreEqual(playerController.runSpeed, 12f);
    }

    [Test]
    public void PlayerCanUseGravityBuff()
    {
        playerController.ApplyGravityBuff(1f, 5f);
        Assert.AreEqual(playerController.Rb.gravityScale, 1f);
    }

    [Test]
    public void PlayerCannotMoveWhenDead()
    {
        playerController.onHit(9999, Vector2.zero);
        Assert.IsFalse(playerController.isMoving);
    }

    [Test]
    public void SetFacingDirection_FlipsCorrectly()
    {
        var player = new GameObject().AddComponent<PlayerController>();
        player.SetDamageable(new Damageable());
        player.SetTouchingDirections(new TouchingDirections());
        player.SetRigidbody(player.gameObject.AddComponent<Rigidbody2D>());

        player._isFacingRight = true;
        var method = player.GetType().GetMethod("setFacingDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        method.Invoke(player, new object[] { new Vector2(-1, 0) });
        Assert.IsFalse(player.isFacingRight);

        method.Invoke(player, new object[] { new Vector2(1, 0) });
        Assert.IsTrue(player.isFacingRight);
    }

    [Test]
    public void RangedAttackCooldown_BlocksSpamming()
    {
        var player = new GameObject().AddComponent<PlayerController>();
        player.SetRigidbody(player.gameObject.AddComponent<Rigidbody2D>());
        var context = new InputAction.CallbackContext();

        player.onRangedAttack(context);

        float firstTime = player.GetType().GetField("lastRangedAttackTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(player) as float? ?? -999f;
        player.onRangedAttack(context);

        float secondTime = player.GetType().GetField("lastRangedAttackTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(player) as float? ?? -999f;
        
        Assert.AreEqual(firstTime, secondTime);
    }

    [Test]
    public void RespawnSetup_DecreasesLifeCount()
    {
        var player = new GameObject().AddComponent<PlayerController>();
        int livesBefore = player.currentLives;

        player.RespawnSetup();

        if (livesBefore > 0)
        {
            Assert.AreEqual(livesBefore - 1, player.currentLives);
        }
    }
}