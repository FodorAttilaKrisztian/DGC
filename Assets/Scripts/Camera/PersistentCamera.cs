using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class PersistentCamera : MonoBehaviour
{
    public static PersistentCamera instance { get; private set; }

    private CinemachineCamera cam;

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        cam = GetComponent<CinemachineCamera>();
        
        if (cam == null)
        {
            Debug.LogWarning("PersistentCamera: CinemachineCamera component is missing. Adding a new one.");
            cam = gameObject.AddComponent<CinemachineCamera>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        UpdateCameraTarget();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCameraTarget();
    }

    public void UpdateCameraTarget()
    {
        if (cam == null)
        {
            Debug.LogWarning("PersistentCamera: Skipping target assignment — no camera.");
            return;
        }

        if (PlayerController.instance != null)
        {
            cam.Follow = PlayerController.instance.transform;
        }
        else
        {
            cam.Follow = null;
            Debug.LogWarning("PersistentCamera: PlayerController instance not found.");
        }
    }

    public CinemachineCamera GetCinemachineCamera()
    {
        return cam;
    }
}