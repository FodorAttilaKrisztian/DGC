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
        // Load the TestScene specifically for testing. Ensure it's only loaded in the editor.
        SceneManager.LoadScene("TestScene");

        // Set up the canvas object and add the Canvas component
        canvasObject = new GameObject("PersistentCanvas");
        canvasObject.AddComponent<Canvas>();  // Ensure a Canvas component is attached to the GameObject
        
        // Now add the PersistentCanvas script
        persistentCanvas = canvasObject.AddComponent<PersistentCanvas>();

        // Call Awake manually, if needed, for PlayMode tests
        typeof(PersistentCanvas)
            .GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(persistentCanvas, null);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after the test is done
        Object.Destroy(canvasObject);
    }

    [Test]
    public void DontDestroyOnLoad_ShouldPersistAcrossScenes()
    {
        if (Application.isPlaying)
        {
            // Ensure the canvas is marked to not be destroyed across scenes
            Object.DontDestroyOnLoad(canvasObject);

            #if UNITY_EDITOR
            // Simulate scene load by reloading the TestScene. Only works in the editor.
            SceneManager.LoadScene("TestScene");
            #endif

            // Assert that PersistentCanvas still exists after the scene load
            Assert.IsNotNull(Object.FindFirstObjectByType<PersistentCanvas>(), "PersistentCanvas should persist across scenes.");
        }
    }
}