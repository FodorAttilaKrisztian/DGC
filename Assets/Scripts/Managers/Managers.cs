using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Managers : MonoBehaviour
{
    public static Managers instance;
    
    public GameObject playerPrefab; // Assign in Inspector
    public GameObject cinemachineCameraPrefab;
    public GameObject gameCanvasPrefab; // Assign in Inspector
    public GameObject sceneManagerPrefab;
    public GameObject uiManagerPrefab;
    public GameObject powerupManagerPrefab;
    public GameObject dataPersistenceManagerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the player across scenes
            EnsurePersistentObjects();
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void EnsurePersistentObjects()
    {
        if (FindFirstObjectByType<PlayerController>() == null)
        {
            Instantiate(playerPrefab);
        }

        if (FindFirstObjectByType<CinemachineCamera>() == null)
        {
            Instantiate(cinemachineCameraPrefab);
        }

        if (FindFirstObjectByType<Canvas>() == null)
        {
            Instantiate(gameCanvasPrefab);
        }

        if (FindFirstObjectByType<SceneController>() == null)
        {
            Instantiate(sceneManagerPrefab);
        }

        if (FindFirstObjectByType<UIManager>() == null)
        {
            Instantiate(uiManagerPrefab);
        }

        if (FindFirstObjectByType<PowerupInventory>() == null)
        {
            Instantiate(powerupManagerPrefab);
        }

        if (FindFirstObjectByType<DataPersistenceManager>() == null)
        {
            Instantiate(dataPersistenceManagerPrefab);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePersistentObjects(); // Check again whenever a new scene loads
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}