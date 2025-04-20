using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class WinMenu : MonoBehaviour
{
    public static WinMenu instance { get; private set; }

    private DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SceneCleanup.DestroyPersistents();

        dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager == null)
        {
            Debug.LogError("DataPersistenceManager is not initialized! Make sure it is added to the scene.");
            
            return;
        }

        dataPersistenceManager.LoadGame(); 
    }

    public void BackToMenu()
    {
        string filePath = Path.Combine(Application.persistentDataPath, DataPersistenceManager.instance.FileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadScene("MainMenu");
    }
}