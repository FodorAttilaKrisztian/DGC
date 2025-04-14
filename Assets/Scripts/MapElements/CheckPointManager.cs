using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private CheckPoint currentCheckpoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void TryActivateCheckpoint(CheckPoint newCheckpoint)
    {
        if (newCheckpoint.IsActivated())
        {
            return;
        }

        if (currentCheckpoint == null || newCheckpoint.priority > currentCheckpoint.priority)
        {
            if (currentCheckpoint != null)
            {
                currentCheckpoint.SetToDefault();
            }

            newCheckpoint.SetToActivated();

            currentCheckpoint = newCheckpoint;

            var player = FindFirstObjectByType<PlayerController>();

            if (player != null)
            {
                player.respawnPoint = newCheckpoint.transform.position;
            }
        }
    }

    public Vector2 GetCurrentCheckpointPosition()
    {
        return currentCheckpoint != null ? currentCheckpoint.transform.position : Vector2.zero;
    }
}