using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string levelName;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> breakablesDestroyed;

    public GameData()
    {
        this.levelName = "Dungeon";
        playerPosition = Vector3.zero;
        breakablesDestroyed = new SerializableDictionary<string, bool>();
    }
}