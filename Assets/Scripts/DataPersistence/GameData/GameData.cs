using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerupData
{
    public string id;
    public string effectName;
    public Vector3 position;

    public PowerupData(string id, string effectName, Vector3 position)
    {
        this.id = id;
        this.effectName = effectName;
        this.position = position;
    }
}

[System.Serializable]
public class GameData
{
    public int currentHP;
    public int currentLifeCount;
    public int breakablesTotal;
    public int eliminationsTotal;
    public int score;
    public int highScore;
    public string levelName;
    public Vector3 lastCheckpointPosition;
    public bool fireballCollected;
    public bool keyCollected;
    public SerializableDictionary<string, bool> breakablesDestroyed;
    public SerializableDictionary<string, bool> livesCollected;
    public List<PowerupData> uncollectedPowerups;
    public SerializableDictionary<string, bool> powerupsCollected;
    public List<string> powerupNames;

    public GameData()
    {
        currentHP = 100;
        currentLifeCount = 3;
        breakablesTotal = 0;
        eliminationsTotal = 0;
        score = 0;
        highScore = 0;
        levelName = "Tutorial";
        lastCheckpointPosition = Vector3.zero;
        fireballCollected = false;
        keyCollected = false;
        breakablesDestroyed = new SerializableDictionary<string, bool>();
        livesCollected = new SerializableDictionary<string, bool>();
        uncollectedPowerups = new List<PowerupData>();
        powerupsCollected = new SerializableDictionary<string, bool>();
        powerupNames = new List<string>();
    }
}