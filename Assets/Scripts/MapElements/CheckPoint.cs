using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite defaultSprite;
    public Sprite activatedSprite;

    [Tooltip("Higher means closer to the goal. Must be unique.")]
    public int priority;

    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetToDefault();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CheckpointManager.Instance?.TryActivateCheckpoint(this);
        }
    }

    public void SetToDefault()
    {
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    public void SetToActivated()
    {
        if (spriteRenderer != null && activatedSprite != null)
        {
            spriteRenderer.sprite = activatedSprite;
        }

        isActivated = true;
        PlayCheckPointSound();
    }

    public bool IsActivated() => isActivated;

    [ContextMenu("Play Checkpoint Sound")]
    public void PlayCheckPointSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.CheckpointSound, 0.5f);
        }
    }
}