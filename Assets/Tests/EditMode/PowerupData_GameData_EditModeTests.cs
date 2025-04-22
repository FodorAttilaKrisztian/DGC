using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PowerupDataTests
{
    [Test]
    public void PowerupData_Constructor_SetsFieldsCorrectly()
    {
        string id = "Powerup01";
        string effectName = "SpeedBoost";
        Vector3 position = new Vector3(10, 5, 0);

        PowerupData powerup = new PowerupData(id, effectName, position);

        Assert.AreEqual(id, powerup.id, "Powerup ID should be set correctly.");
        Assert.AreEqual(effectName, powerup.effectName, "Powerup effect name should be set correctly.");
        Assert.AreEqual(position, powerup.position, "Powerup position should be set correctly.");
    }
}

public class GameDataTests
{
    [Test]
    public void GameData_Constructor_SetsFieldsCorrectly()
    {
        GameData gameData = new GameData();

        Assert.AreEqual(100, gameData.currentHP, "Current HP should be initialized to 100.");
        Assert.AreEqual(3, gameData.currentLifeCount, "Life count should be initialized to 3.");
        Assert.AreEqual(0, gameData.breakablesTotal, "Breakables total should be initialized to 0.");
        Assert.AreEqual(0, gameData.eliminationsTotal, "Eliminations total should be initialized to 0.");
        Assert.AreEqual(0, gameData.score, "Score should be initialized to 0.");
        Assert.AreEqual(0, gameData.highScore, "High score should be initialized to 0.");
        Assert.AreEqual("Tutorial", gameData.levelName, "Level name should be initialized to 'Tutorial'.");
        Assert.AreEqual(Vector3.zero, gameData.lastCheckpointPosition, "Last checkpoint position should be initialized to Vector3.zero.");
        Assert.IsFalse(gameData.fireballCollected, "Fireball collected should be initialized to false.");
        Assert.IsFalse(gameData.keyCollected, "Key collected should be initialized to false.");
        Assert.IsEmpty(gameData.breakablesDestroyed, "Breakables destroyed dictionary should be empty.");
        Assert.IsEmpty(gameData.livesCollected, "Lives collected dictionary should be empty.");
        Assert.IsEmpty(gameData.uncollectedPowerups, "Uncollected powerups list should be empty.");
        Assert.IsEmpty(gameData.powerupsCollected, "Powerups collected dictionary should be empty.");
        Assert.IsEmpty(gameData.powerupNames, "Powerup names list should be empty.");
    }
}