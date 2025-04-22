using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;

public class DataPersistenceManagerPlayModeTests
{
    private GameObject gameObject;
    private DataPersistenceManager dataPersistenceManager;
    private string testFilePath;

    [SetUp]
    public void Setup()
    {
        // Ensure no previous instance exists
        if (DataPersistenceManager.instance != null)
        {
            Object.Destroy(DataPersistenceManager.instance.gameObject);
        }

        // Create a new GameObject with the DataPersistenceManager attached to it
        gameObject = new GameObject();
        dataPersistenceManager = gameObject.AddComponent<DataPersistenceManager>();

        // Manually initialize the dataHandler here
        testFilePath = Path.Combine(Application.persistentDataPath, "testSaveData.json");
        dataPersistenceManager.GetType().GetField("dataHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(dataPersistenceManager, new FileDataHandler(Application.persistentDataPath, "testSaveData.json", false));

        // Set the FileName for the test
        dataPersistenceManager.FileName = "testSaveData.json";

        // Add a mock IDataPersistence object directly to the GameObject
        gameObject.AddComponent<MockDataPersistence>();  // Add the mock component here
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);  // Delete any leftover save file
        }

        Object.Destroy(gameObject);
    }

    [Test]
    public void DataPersistenceManager_Singleton_IsUnique()
    {
        // Test that only one instance of DataPersistenceManager exists in the scene.
        DataPersistenceManager instance1 = DataPersistenceManager.instance;
        GameObject gameObject2 = new GameObject();
        DataPersistenceManager instance2 = gameObject2.AddComponent<DataPersistenceManager>();

        // Should destroy the second instance and keep the first one
        Assert.AreEqual(instance1, DataPersistenceManager.instance);

        Object.Destroy(gameObject2);
    }

    [Test]
    public void NewGame_InitializesDefaultGameData()
    {
        // Test that the NewGame function initializes the game data with default values.
        dataPersistenceManager.NewGame();
        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP);
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount);
        Assert.AreEqual("Tutorial", dataPersistenceManager.GameData.levelName);
    }

    [UnityTest]
    public IEnumerator SaveGame_SavesDataCorrectly()
    {
        // Initialize new game data
        dataPersistenceManager.NewGame();

        // Ensure fileName is set
        dataPersistenceManager.FileName = "testSaveData.json";

        // Check that game data is not null
        Assert.IsNotNull(dataPersistenceManager.GameData);

        // Save the game
        dataPersistenceManager.SaveGame();

        // Check if the file exists after saving
        string filePath = Path.Combine(Application.persistentDataPath, dataPersistenceManager.FileName);
        yield return new WaitForEndOfFrame();  // Wait for file write operation to complete

        // Assert that the file exists
        Assert.IsTrue(File.Exists(filePath));

        // Clean up
        File.Delete(filePath);
    }

    [UnityTest]
    public IEnumerator LoadGame_InitializesNewGameIfNoDataExists()
    {
        // Ensure the save file doesn't exist
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath); // Delete the save file if it exists
        }

        // Load the game (which should initialize new data if no data exists)
        dataPersistenceManager.LoadGame();

        // Check that the game data has been initialized with default values
        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP); // Check initial HP
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount); // Check initial life count
        Assert.AreEqual("Tutorial", dataPersistenceManager.GameData.levelName); // Check initial level name

        yield return null;
    }

    // Mock IDataPersistence to simulate saving and loading data
    public class MockDataPersistence : MonoBehaviour, IDataPersistence
    {
        public void LoadData(GameData gameData)
        {
            // Mock Load - no actual data loading (will leave gameData unmodified)
        }

        public void SaveData(ref GameData gameData)
        {
            // Mock Save - no actual data saving (doesn't modify gameData)
        }
    }
}