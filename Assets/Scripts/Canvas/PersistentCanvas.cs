using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas instance;
    private UIManager uiManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive between scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates when loading new scenes
        
            return;
        }

        uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("PersistentCanvas: UIManager component missing!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (uiManager != null)
        {
            uiManager.ResetUI();
        }
    }
}