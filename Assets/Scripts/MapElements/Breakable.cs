using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Breakable : MonoBehaviour, IDataPersistence
{
    private AudioManager audioManager;
    private Animator animator;

    [SerializeField]
    private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public GameObject[] allPowerups;
    private Damageable damageable;
    private bool hasDroppedPowerup = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioManager = AudioManager.instance;

        damageable = GetComponent<Damageable>();

        if (damageable != null)
        {
            damageable.maxHealth = 1;
            damageable.health = 1;
        }
    }

    private void Start()
    {
        damageable.damageableHit.AddListener(OnHit);
    }

    public void OnHit(int damage, Vector2 knockback)
    {   
        if (damageable.health <= 0 && !hasDroppedPowerup)
        {
            animator.SetBool(AnimationStrings.isAlive, false);
            
            DropPowerup();
            hasDroppedPowerup = true;

            DataPersistenceManager.instance.SaveGame();

            StartCoroutine(WaitandDestroy());
        }
    }

    private void DropPowerup()
    {
        GameObject chosenPowerup = GetRandomValidPowerup();

        if (chosenPowerup != null)
        {
            GameObject spawned = Instantiate(chosenPowerup, transform.position, Quaternion.identity);

            Powerup powerupComponent = spawned.GetComponent<Powerup>();

            if (powerupComponent != null)
            {
                // Combine the breakable ID + effect type to make a consistent unique ID
                powerupComponent.InitializePersistentID(id + "_" + powerupComponent.effect.name);
            }
        }
    }

    private GameObject GetRandomValidPowerup()
    {   
        PlayerController player = FindFirstObjectByType<PlayerController>();

        List<GameObject> validPowerups = new List<GameObject>();

        foreach (GameObject powerup in allPowerups)
        {
            Powerup powerupComponent = powerup.GetComponent<Powerup>();

            if (powerupComponent != null && IsValidPowerup(powerupComponent, player))
            {
                validPowerups.Add(powerup);
            }
        }

        if (validPowerups.Count > 0)
        {
            return validPowerups[Random.Range(0, validPowerups.Count)];
        }

        return null;
    }

    private bool IsValidPowerup(Powerup powerup, PlayerController player)
    {
        string powerupName = powerup.effect.name;

        if (powerupName == "SmallHealthBuff" || powerupName == "LargeHealthBuff")
        {
            Damageable playerDamageable = player.GetComponent<Damageable>();
            
            if (playerDamageable != null && playerDamageable.health >= playerDamageable.maxHealth)
            {
                return false;
            }
        }

        return true;
    }

    public void LoadData(GameData data)
    {
        if (data.breakablesDestroyed.TryGetValue(id, out hasDroppedPowerup) && hasDroppedPowerup)
        {
            if (this == null || gameObject == null) 
            {
                Debug.LogWarning("Breakable object is already destroyed.");
                
                return;
            }

            Destroy(gameObject);
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.breakablesDestroyed.ContainsKey(id))
        {
            data.breakablesDestroyed.Remove(id);
        }

        data.breakablesDestroyed.Add(id, hasDroppedPowerup);
    }

    public void PlayBoxSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.boxBreakSound, 0.25f);
        }
    }

    public void PlayVaseSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.vaseBreakSound, 0.25f);
        }
    }

    public void PlayBarrelSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.barrelBreakSound, 0.25f);
        }
    }

    private IEnumerator WaitandDestroy()
    {
        yield return new WaitForSeconds(0.005f);

        Destroy(gameObject);
    }
}