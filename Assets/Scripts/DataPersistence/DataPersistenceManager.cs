using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager instance { get; private set; }

    [Header("File Storage Config")]
    
    [SerializeField] private string fileName = "defaultSaveFile.json";

    [SerializeField] private bool useEncryption;

    private GameData gameData; 
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public string FileName
    {
        get => fileName;
        set => fileName = value; // Allow test code to assign the file name
    }

    public GameData GameData
    {
        get => gameData;
        set => gameData = value;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        StartCoroutine(InitializeDataPersistence());
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {

        if (dataHandler == null)
        {
            Debug.LogError("Data handler is not initialized.");
            return;
        }
        
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogWarning("No IDataPersistence objects found in the scene.");
            return;
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public virtual void SaveGame()
    {
        if (gameData == null) return;

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogWarning("No data persistence objects found.");
            return;
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnDestroy()
    {
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private IEnumerator InitializeDataPersistence()
    {
        yield return new WaitForEndOfFrame(); // Wait until the first frame is done rendering
        LoadGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        if (dataPersistenceObjects == null || !dataPersistenceObjects.Any())
        {
            Debug.LogWarning("No IDataPersistence objects found.");
        }

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    #if UNITY_EDITOR || TEST_MODE
    // Test hook to set the instance for unit tests
    public static void SetInstanceForTesting(DataPersistenceManager testInstance)
    {
        instance = testInstance;
    }
    #endif
}