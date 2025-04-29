using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; 
using System.IO;
using System;

public class SceneController : MonoBehaviour
{
    public static SceneController instance { get; private set; }
    
    [SerializeField] 
    private Animator transitionAnim;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializeTransitionAnimator();
    }

    private void InitializeTransitionAnimator()
    {
        if (transitionAnim == null)
        {
            transitionAnim = GetComponentInChildren<Animator>();
            
            if (transitionAnim == null)
            {
                Debug.LogError("SceneController: No Animator found! Transitions won't work.");
            }
        }
    }

    private void Start()
    {
        UIManager.instance?.ResetUI();
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        yield return TriggerLevelEndTransition();

        HandleGameData();

        yield return LoadNextSceneAsync();

        SetPlayerSpawnPosition();

        DataPersistenceManager.instance?.LoadGame();

        TriggerLevelStartTransition();
    }

    private IEnumerator TriggerLevelEndTransition()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelEndTrigger);
            yield return new WaitForSeconds(1f);
        }

        yield break;
    }

    private IEnumerator WaitForTransition(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    private void HandleGameData()
    {
        var dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager != null)
        {
            var gameData = dataPersistenceManager.GameData;

            if (gameData != null)
            {
                gameData.uncollectedPowerups = new List<PowerupData>();

                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    UIManager.instance?.InitializePowerupUI();
                    HandleTutorialGameData(dataPersistenceManager);
                }
                else
                {
                    dataPersistenceManager.SaveGame();
                }
            } 
            else
            {
                Debug.Log("GameData is null or not in the scene. No action taken.");
            }
        }
        else
        {
            Debug.LogError("DataPersistenceManager instance is null. Cannot handle game data.");
        }
    }

    private void HandleTutorialGameData(DataPersistenceManager dataPersistenceManager)
    {
        string filePath = Path.Combine(Application.persistentDataPath, dataPersistenceManager.FileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        dataPersistenceManager.NewGame();
    }

    private IEnumerator LoadNextSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private void SetPlayerSpawnPosition()
    {
        PlayerController player = PlayerController.instance;

        if (player.respawnPoint == Vector3.zero)
        {
            SetPlayerToCheckpoint(player);
        }
        else
        {
            player.transform.position = player.respawnPoint;
        }
    }

    private void SetPlayerToCheckpoint(PlayerController player)
    {
        CheckPoint[] allCheckpoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);

        if (allCheckpoints != null && allCheckpoints.Length > 0)
        {
            CheckPoint lowest = allCheckpoints[0];

            foreach (CheckPoint cp in allCheckpoints)
            {
                if (cp.priority < lowest.priority)
                {
                    lowest = cp;
                }
            }

            player.transform.position = lowest.transform.position;
            player.respawnPoint = lowest.transform.position;
        }
        else
        {
            Debug.LogWarning("No checkpoints found in scene.");
        }
    }

    private void TriggerLevelStartTransition()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelStartTrigger);
        }
    }
}