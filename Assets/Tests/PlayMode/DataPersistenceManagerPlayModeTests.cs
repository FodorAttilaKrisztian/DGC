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
        if (DataPersistenceManager.instance != null)
        {
            Object.Destroy(DataPersistenceManager.instance.gameObject);
        }

        gameObject = new GameObject();
        dataPersistenceManager = gameObject.AddComponent<DataPersistenceManager>();

        testFilePath = Path.Combine(Application.persistentDataPath, "testSaveData.json");
        dataPersistenceManager.GetType().GetField("dataHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(dataPersistenceManager, new FileDataHandler(Application.persistentDataPath, "testSaveData.json", false));

        dataPersistenceManager.FileName = "testSaveData.json";

        gameObject.AddComponent<MockDataPersistence>();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }

        Object.Destroy(gameObject);
    }

    [Test]
    public void DataPersistenceManager_Singleton_IsUnique()
    {
        DataPersistenceManager instance1 = DataPersistenceManager.instance;
        GameObject gameObject2 = new GameObject();
        DataPersistenceManager instance2 = gameObject2.AddComponent<DataPersistenceManager>();

        Assert.AreEqual(instance1, DataPersistenceManager.instance);

        Object.Destroy(gameObject2);
    }

    [Test]
    public void NewGame_InitializesDefaultGameData()
    {
        dataPersistenceManager.NewGame();
        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP);
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount);
        Assert.AreEqual("Tutorial", dataPersistenceManager.GameData.levelName);
    }

    [UnityTest]
    public IEnumerator SaveGame_SavesDataCorrectly()
    {
        dataPersistenceManager.NewGame();

        dataPersistenceManager.FileName = "testSaveData.json";

        Assert.IsNotNull(dataPersistenceManager.GameData);

        dataPersistenceManager.SaveGame();

        string filePath = Path.Combine(Application.persistentDataPath, dataPersistenceManager.FileName);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(File.Exists(filePath));

        File.Delete(filePath);
    }

    [UnityTest]
    public IEnumerator LoadGame_InitializesNewGameIfNoDataExists()
    {
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }

        dataPersistenceManager.LoadGame();

        Assert.AreEqual(100, dataPersistenceManager.GameData.currentHP);
        Assert.AreEqual(3, dataPersistenceManager.GameData.currentLifeCount);
        Assert.AreEqual("Tutorial", dataPersistenceManager.GameData.levelName);

        yield return null;
    }

    public class MockDataPersistence : MonoBehaviour, IDataPersistence
    {
        public void LoadData(GameData gameData)
        {}

        public void SaveData(ref GameData gameData)
        {}
    }
}