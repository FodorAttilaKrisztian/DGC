using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Managers : MonoBehaviour
{
    public static Managers instance { get; private set; }

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cinemachineCameraPrefab;
    [SerializeField] private GameObject gameCanvasPrefab;
    [SerializeField] private GameObject sceneManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private GameObject powerupManagerPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsurePersistentObjects();
    }

    private void EnsurePersistentObjects()
    {
        InstantiateIfNotFound<PlayerController>(playerPrefab);
        InstantiateIfNotFound<CinemachineCamera>(cinemachineCameraPrefab);
        
        // Canvas nem MonoBehaviour
        if (FindFirstObjectByType<Canvas>() == null)
        {
            Instantiate(gameCanvasPrefab);
        }

        InstantiateIfNotFound<SceneController>(sceneManagerPrefab);
        InstantiateIfNotFound<UIManager>(uiManagerPrefab);
        InstantiateIfNotFound<PowerupInventory>(powerupManagerPrefab);
    }

    private void InstantiateIfNotFound<T>(GameObject prefab) where T : MonoBehaviour
    {
        if (FindFirstObjectByType<T>() == null)
        {
            Instantiate(prefab);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePersistentObjects();
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