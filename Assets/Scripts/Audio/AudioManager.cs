using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip dungeonMusic;
    [SerializeField] private AudioClip villageMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip singlePunchSound;
    [SerializeField] private AudioClip doublePunchSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip gameOverSound;

    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip speedSound;
    [SerializeField] private AudioClip gravitySound;

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField] private AudioClip checkpointSound;
    [SerializeField] private AudioClip boxBreakSound;
    [SerializeField] private AudioClip vaseBreakSound;
    [SerializeField] private AudioClip barrelBreakSound;

    [SerializeField] private AudioClip rockThrowSound;
    [SerializeField] private AudioClip rockHitSound;
    [SerializeField] private AudioClip fireballThrowSound;
    [SerializeField] private AudioClip fireballHitSound;

    [SerializeField] private AudioClip skeletonHurtSound;
    [SerializeField] private AudioClip skeletonDeathSound;
    [SerializeField] private AudioClip skeletonSwordSound;
    [SerializeField] private AudioClip skeletonHammerSound;
    [SerializeField] private AudioClip skeletonKickSound;

    #region Public Getters for SFX (used by other classes)

    public AudioClip SinglePunchSound => singlePunchSound;
    public AudioClip DoublePunchSound => doublePunchSound;
    public AudioClip JumpSound => jumpSound;
    public AudioClip PlayerHurtSound => playerHurtSound;
    public AudioClip DeathSound => deathSound;
    public AudioClip GameOverSound => gameOverSound;

    public AudioClip HealSound => healSound;
    public AudioClip SpeedSound => speedSound;
    public AudioClip GravitySound => gravitySound;

    public AudioClip PickupSound => pickupSound;
    public AudioClip KeyPickupSound => keyPickupSound;
    public AudioClip CheckpointSound => checkpointSound;
    public AudioClip BoxBreakSound => boxBreakSound;
    public AudioClip VaseBreakSound => vaseBreakSound;
    public AudioClip BarrelBreakSound => barrelBreakSound;

    public AudioClip RockThrowSound => rockThrowSound;
    public AudioClip RockHitSound => rockHitSound;
    public AudioClip FireballThrowSound => fireballThrowSound;
    public AudioClip FireballHitSound => fireballHitSound;

    public AudioClip SkeletonHurtSound => skeletonHurtSound;
    public AudioClip SkeletonDeathSound => skeletonDeathSound;
    public AudioClip SkeletonSwordSound => skeletonSwordSound;
    public AudioClip SkeletonHammerSound => skeletonHammerSound;
    public AudioClip SkeletonKickSound => skeletonKickSound;

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
            case "Tutorial":
            case "Win":
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
        if (musicSource.clip == newClip) return;

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}