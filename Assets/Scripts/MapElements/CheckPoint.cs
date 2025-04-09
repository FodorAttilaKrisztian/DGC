using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    public Sprite defaultSprite;
    public Sprite activatedSprite;

    AudioManager audioManager;

    private static CheckPoint currentActiveCheckpoint;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        playerController = FindFirstObjectByType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetToDefault();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && playerController != null)
        {
            // Check if the player is not already at the checkpoint
            if (playerController.respawnPoint.x < transform.position.x)
            {
                playerController.respawnPoint = transform.position;

                // Reset the previous checkpoint if it exists
                if (currentActiveCheckpoint != null)
                {
                    currentActiveCheckpoint.SetToDefault();
                }

                // Activate this one
                SetToActivated();

                currentActiveCheckpoint = this;
            }
        }
    }

    private void SetToDefault()
    {
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void SetToActivated()
    {
        if (spriteRenderer != null && activatedSprite != null)
        {
            spriteRenderer.sprite = activatedSprite;
            PlayCheckPointSound();
        }
    }

    [ContextMenu("Sound Effects")]
    public void PlayCheckPointSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.checkpointSound, 0.5f);
        }
    }
}