using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; 
using System.IO;
using System;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    
    [SerializeField] 
    private Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }
        
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
        if (UIManager.instance != null)
        {
            UIManager.instance.ResetUI();
        }
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelEndTrigger);
            yield return new WaitForSeconds(1);
        }

        DataPersistenceManager dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager != null)
        {
            GameData gameData = dataPersistenceManager.GameData;

            if (gameData != null)
            {
                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    if (UIManager.instance != null)
                    {
                        UIManager.instance.InitializePowerupUI();
                    }

                    string filePath = Path.Combine(Application.persistentDataPath, dataPersistenceManager.FileName);
        
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    dataPersistenceManager.NewGame();
                }
            }
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        
        while (!operation.isDone)
        {
            yield return null; // Wait until the scene is fully loaded
        }

        PlayerController player = PlayerController.instance;

        if (player.respawnPoint == Vector3.zero)
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
        else
        {
            player.transform.position = player.respawnPoint;
        }

        if (dataPersistenceManager != null)
        {
            dataPersistenceManager.LoadGame();
        }

        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelStartTrigger);
        }
    }
}