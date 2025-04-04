using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class PersistentCamera : MonoBehaviour
{
    public static PersistentCamera instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void Start()
    {
        UpdateCameraTarget();
    }

    private void UpdateCameraTarget()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        
        if (player != null)
        {
            GetComponent<CinemachineCamera>().Follow = player.transform;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += (scene, mode) => UpdateCameraTarget();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= (scene, mode) => UpdateCameraTarget();
    }
}