using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas instance;
    private Canvas canvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        
            return;
        }

        canvas = GetComponent<Canvas>();
    }

    public Canvas GetCanvas()
    {
        return canvas;
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
        if (UIManager.instance != null)
        {
            UIManager.instance.ResetUI();
        }
    }
}