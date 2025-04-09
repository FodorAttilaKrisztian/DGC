using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    public Sprite defaultSprite;
    public Sprite activatedSprite;

    private static CheckPoint currentActiveCheckpoint;

    private void Awake()
    {
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
            if (playerController.respawnPoint.x < transform.position.x)
            {
                playerController.respawnPoint = transform.position;

                if (currentActiveCheckpoint != null)
                {
                    currentActiveCheckpoint.SetToDefault();
                }

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
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.checkpointSound, 0.5f);
        }
    }
}