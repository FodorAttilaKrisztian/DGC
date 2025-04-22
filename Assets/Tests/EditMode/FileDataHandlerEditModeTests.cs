using NUnit.Framework;
using System.IO;
using UnityEngine;

public class FileDataHandlerTests
{
    private string testPath;
    private string testFileName;
    private FileDataHandler fileDataHandler;

    [SetUp]
    public void Setup()
    {
        testPath = Path.Combine(Application.persistentDataPath, "testDir");
        testFileName = "testSaveData.json";
        fileDataHandler = new FileDataHandler(testPath, testFileName, false); // No encryption for simplicity
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        string fullPath = Path.Combine(testPath, testFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        if (Directory.Exists(testPath))
        {
            Directory.Delete(testPath);
        }
    }

    [Test]
    public void SaveGame_CreatesFile()
    {
        GameData gameData = new GameData { currentHP = 100, currentLifeCount = 3, levelName = "TestLevel" };

        fileDataHandler.Save(gameData);

        string fullPath = Path.Combine(testPath, testFileName);
        Assert.IsTrue(File.Exists(fullPath), "Save file should be created.");
    }

    [Test]
    public void LoadGame_ReturnsCorrectData()
    {
        GameData expectedGameData = new GameData { currentHP = 100, currentLifeCount = 3, levelName = "TestLevel" };
        fileDataHandler.Save(expectedGameData);

        GameData loadedGameData = fileDataHandler.Load();

        Assert.AreEqual(expectedGameData.currentHP, loadedGameData.currentHP);
        Assert.AreEqual(expectedGameData.currentLifeCount, loadedGameData.currentLifeCount);
        Assert.AreEqual(expectedGameData.levelName, loadedGameData.levelName);
    }

    [Test]
    public void LoadGame_ReturnsNullWhenFileDoesNotExist()
    {
        // Ensure the file doesn't exist before the test
        string fullPath = Path.Combine(testPath, testFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        GameData loadedGameData = fileDataHandler.Load();

        Assert.IsNull(loadedGameData, "Expected null when no save file exists.");
    }

    [Test]
    public void SaveGame_HandlesFileWriteError()
    {
        // Simulate a write error by providing an invalid directory
        string invalidPath = "/invalid/directory";
        FileDataHandler faultyFileDataHandler = new FileDataHandler(invalidPath, testFileName, false);

        GameData gameData = new GameData { currentHP = 100, currentLifeCount = 3, levelName = "TestLevel" };

        Assert.DoesNotThrow(() => faultyFileDataHandler.Save(gameData), "Save should not throw an error even with an invalid path.");
    }
}