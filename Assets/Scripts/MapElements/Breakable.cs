using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Breakable : MonoBehaviour, IDataPersistence
{
    private AudioManager audioManager;
    private Animator animator;
    private DataPersistenceManager dataPersistenceManager;

    [SerializeField]
    private string id;

    public string GetID()
    {
        return id;
    }

    [ContextMenu("Generate guid for id")]
    public void GenerateGuid()
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

        dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager == null)
        {
            Debug.LogWarning($"[Breakable:{gameObject.name}] Start(): DataPersistenceManager.instance is STILL null!");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {   
        if (damageable.health <= 0 && !hasDroppedPowerup)
        {
            animator.SetBool(AnimationStrings.isAlive, false);
            
            hasDroppedPowerup = true;

            DropPowerup();

            dataPersistenceManager.SaveGame();

            StartCoroutine(WaitandDestroy());
        }
    }

    private void DropPowerup()
    {
        if (Random.value <= 0.5f)
        {
            return;
        }

        GameObject chosenPowerup = GetRandomValidPowerup();

        if (chosenPowerup != null)
        {
            GameObject spawned = Instantiate(chosenPowerup, transform.position, Quaternion.identity);

            Powerup powerupComponent = spawned.GetComponent<Powerup>();

            if (powerupComponent != null)
            {
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

            if (powerupComponent != null)
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
            audioManager.PlaySFX(audioManager.BoxBreakSound, 0.25f);
        }
    }

    public void PlayVaseSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.VaseBreakSound, 0.25f);
        }
    }

    public void PlayBarrelSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.BarrelBreakSound, 0.25f);
        }
    }

    private IEnumerator WaitandDestroy()
    {
        yield return new WaitForSeconds(0.005f);

        Destroy(gameObject);
    }

    public void AddToBreakableCounter()
    {
        if (dataPersistenceManager == null)
        {
            Debug.LogWarning("[Breakable] DataPersistenceManager is NULL.");
            return;
        }

        GameData gameData = dataPersistenceManager.GameData;

        if (gameData == null)
        {
            Debug.LogWarning("[Breakable] GameData is NULL.");
            return;
        }

        gameData.breakablesTotal++;
        
        gameData.score += 50;

        dataPersistenceManager.SaveGame();
    }

    #if UNITY_EDITOR || TEST_MODE
    public void SetDataPersistenceManagerForTesting(DataPersistenceManager manager)
    {
        this.dataPersistenceManager = manager;
    }
    #endif
}