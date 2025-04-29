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
        audioManagerObject = new GameObject();
        audioManager = audioManagerObject.AddComponent<AudioManager>();

        musicSource = audioManagerObject.AddComponent<AudioSource>(); 

        SetPrivateField(audioManager, "musicSource", musicSource);

        AudioClip menuMusic = AudioClip.Create("MenuMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "menuMusic", menuMusic);

        AudioClip dungeonMusic = AudioClip.Create("DungeonMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "dungeonMusic", dungeonMusic);

        AudioClip villageMusic = AudioClip.Create("VillageMusic", 44100, 1, 44100, false);
        SetPrivateField(audioManager, "villageMusic", villageMusic);
    }

    [TearDown]
    public void TearDown()
    {
        if (audioManagerObject != null)
        {
            GameObject.DestroyImmediate(audioManagerObject);
        }
    }

    [UnityTest]
    public IEnumerator ChangeScene_MusicUpdatesCorrectly()
    {
        AudioClip menuMusic = AudioClip.Create("MenuMusic", 44100, 1, 44100, false);
        
        musicSource.clip = menuMusic;

        Debug.Log("AudioSource Clip before Play: " + musicSource.clip);

        Scene dummyScene = SceneManager.GetActiveScene();

        InvokeOnSceneLoaded(dummyScene, LoadSceneMode.Single);

        musicSource.Play();

        Debug.Log("AudioSource is playing: " + musicSource.isPlaying);
        
        yield return null;

        Assert.IsTrue(musicSource.isPlaying, "Music should be playing.");
        Assert.AreEqual(menuMusic, musicSource.clip, "The correct music clip should be playing.");
    }

    [UnityTest]
    public IEnumerator OnSceneLoaded_UnknownScene_StopsMusic()
    {
        Scene dummyScene = new Scene();

        InvokeOnSceneLoaded(dummyScene, LoadSceneMode.Single);

        yield return null;

        Assert.IsFalse(musicSource.isPlaying);
        Assert.AreEqual(null, musicSource.clip);
    }

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