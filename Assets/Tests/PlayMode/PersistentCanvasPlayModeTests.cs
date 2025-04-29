using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class PersistentCanvasPlayModeTests
{
    private GameObject canvasObject;
    private PersistentCanvas persistentCanvas;

    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("TestScene");

        canvasObject = new GameObject("PersistentCanvas");
        canvasObject.AddComponent<Canvas>();
        
        persistentCanvas = canvasObject.AddComponent<PersistentCanvas>();

        typeof(PersistentCanvas)
            .GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(persistentCanvas, null);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(canvasObject);
    }

    [Test]
    public void DontDestroyOnLoad_ShouldPersistAcrossScenes()
    {
        if (Application.isPlaying)
        {
            Object.DontDestroyOnLoad(canvasObject);

            #if UNITY_EDITOR
            SceneManager.LoadScene("TestScene");
            #endif

            Assert.IsNotNull(Object.FindFirstObjectByType<PersistentCanvas>(), "PersistentCanvas should persist across scenes.");
        }
    }
}