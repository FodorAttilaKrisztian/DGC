using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip menuMusic;
    public AudioClip dungeonMusic;
    public AudioClip villageMusic;
    public AudioClip checkpointSound;
    public AudioClip deathSound;
    public AudioClip gameOverSound;
    public AudioClip skeletonSwordSound;
    public AudioClip skeletonHammerSound;
    public AudioClip skeletonKickSound;
    public AudioClip singlePunchSound;
    public AudioClip doublePunchSound;
    public AudioClip jumpSound;
    public AudioClip pickupSound;
    public AudioClip keyPickupSound;
    public AudioClip rockThrowSound;
    public AudioClip rockHitSound;
    public AudioClip fireballThrowSound;
    public AudioClip fireballHitSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                ChangeMusic(menuMusic);
                break;
            case "Dungeon":
                ChangeMusic(dungeonMusic);
                break;
            case "Village":
                ChangeMusic(villageMusic);
                break;
            default:
                musicSource.Stop();
                break;
        }
    }

    private void ChangeMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip)
            return;

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(clip, volume);
        }
    }
}