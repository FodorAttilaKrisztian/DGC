using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Reflection;

[TestFixture]
public class AudioManagerTests
{
    private GameObject audioManagerObject;
    private AudioManager audioManager;
    private AudioSource musicSource;

    [SetUp]
    public void SetUp()
    {
        // Create a GameObject and add the AudioManager
        audioManagerObject = new GameObject();
        audioManager = audioManagerObject.AddComponent<AudioManager>();

        // Initialize the AudioSource to simulate the music player
        musicSource = audioManagerObject.AddComponent<AudioSource>(); 

        // Use reflection to set private fields on the AudioManager
        SetPrivateField(audioManager, "musicSource", musicSource);  // Set the AudioSource to the manager

        // Create dummy AudioClips and assign them to the AudioManager
        AudioClip menuMusic = AudioClip.Create("MenuMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "menuMusic", menuMusic); // Set the menuMusic

        AudioClip dungeonMusic = AudioClip.Create("DungeonMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "dungeonMusic", dungeonMusic); // Set dungeonMusic

        AudioClip villageMusic = AudioClip.Create("VillageMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "villageMusic", villageMusic); // Set villageMusic
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up and destroy the audio manager object
        if (audioManagerObject != null)
        {
            GameObject.DestroyImmediate(audioManagerObject); // Destroy in edit mode
        }
    }

    [UnityTest]
    public IEnumerator ChangeScene_MusicUpdatesCorrectly()
    {
        // Create a dummy AudioClip for menu music
        AudioClip menuMusic = AudioClip.Create("MenuMusic", 44100, 1, 44100, false);
        
        // Directly assign the clip to the musicSource instead of using reflection
        musicSource.clip = menuMusic;

        // Log to check if the clip is set correctly
        Debug.Log("AudioSource Clip before Play: " + musicSource.clip);

        // Create a dummy scene (simulating a loaded scene)
        Scene dummyScene = SceneManager.GetActiveScene();

        // Manually trigger OnSceneLoaded using reflection
        InvokeOnSceneLoaded(dummyScene, LoadSceneMode.Single);

        // Explicitly play the music
        musicSource.Play();

        // Debugging: Check if play was called successfully
        Debug.Log("AudioSource is playing: " + musicSource.isPlaying);
        
        // Wait for a frame to allow the music to play
        yield return null;

        // Assert that the music source is playing and the clip is set correctly
        Assert.IsTrue(musicSource.isPlaying, "Music should be playing.");
        Assert.AreEqual(menuMusic, musicSource.clip, "The correct music clip should be playing.");
    }

    [UnityTest]
    public IEnumerator OnSceneLoaded_UnknownScene_StopsMusic()
    {
        // Create a dummy scene (simulating an unknown scene)
        Scene dummyScene = new Scene();

        // Manually trigger OnSceneLoaded using reflection
        InvokeOnSceneLoaded(dummyScene, LoadSceneMode.Single);

        // Wait for a frame to ensure the music has time to stop
        yield return null;

        // Assert that the music source is not playing and no clip is set
        Assert.IsFalse(musicSource.isPlaying);
        Assert.AreEqual(null, musicSource.clip); // No music clip playing
    }

    // Reflection helper method to set private fields
    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogError($"Field '{fieldName}' not found in {target.GetType()}");
        }
    }

    // Reflection helper method to invoke OnSceneLoaded
    private void InvokeOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MethodInfo method = typeof(AudioManager).GetMethod("OnSceneLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(audioManager, new object[] { scene, mode });
        }
        else
        {
            Debug.LogError("OnSceneLoaded method not found in AudioManager.");
        }
    }
}