using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas instance { get; private set; }
    private Canvas canvas;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        canvas = GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("PersistentCanvas: No Canvas component found on this GameObject.");
        }
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.ResetUI();
        }
        else
        {
            Debug.LogWarning("PersistentCanvas: UIManager instance not found during scene load.");
        }
    }
}