using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject managersPrefab;

    private DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneCleanup.DestroyPersistents();

        dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager == null)
        {
            Debug.LogError("DataPersistenceManager is not initialized! Make sure it is added to the scene.");
            
            return;
        }

        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void NewGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, dataPersistenceManager.FileName);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        dataPersistenceManager.NewGame();

        SceneManager.LoadScene("Tutorial");
    }

    public void ContinueGame()
    {
        dataPersistenceManager.LoadGame();

        if (dataPersistenceManager.GameData == null)
        {
            Debug.Log("No existing save data found, starting a new game.");
            NewGame(); 
        }
        else
        {
            SceneManager.LoadScene(dataPersistenceManager.GameData.levelName);
        }
    }

    public void QuitGame()
    {
        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
        #endif

        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE)
            Application.Quit();
        #elif (UNITY_WEBGL)
            SceneManager.LoadScene("QuitScene");
        #endif
    }
}