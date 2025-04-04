using UnityEngine;

public static class PowerupUtils
{
    public static GameObject LoadPowerupPrefab(string powerupName)
    {
        return Resources.Load<GameObject>($"PowerupPickups/{powerupName}");
    }

    public static Sprite GetPowerupSprite(string powerupName)
    {
        GameObject prefab = LoadPowerupPrefab(powerupName);

        if (prefab != null)
        {
            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                return renderer.sprite;
            }
        }

        Debug.LogWarning($"Sprite not found for powerup: {powerupName}");
        
        return null;
    }
}