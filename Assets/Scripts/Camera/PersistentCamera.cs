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
        var cam = GetComponent<CinemachineCamera>();
        
        if (PlayerController.instance == null)
        {
            Debug.LogWarning("PlayerController instance not found.");
            return;
        }

        if (cam == null)
        {
            Debug.LogError("CinemachineCamera component is missing on this GameObject.");
            return;
        }

        cam.Follow = PlayerController.instance.transform;
    }


    private void OnEnable()
    {
        if (PlayerController.instance != null)
        {
            SceneManager.sceneLoaded += (scene, mode) => UpdateCameraTarget();
        }
    }

    private void OnDisable()
    {
        if (PlayerController.instance != null)
        {
            SceneManager.sceneLoaded -= (scene, mode) => UpdateCameraTarget();
        }
    }
}