using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;

public class WinMenu : MonoBehaviour
{
    public static WinMenu instance { get; private set; }
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() 
    {
        if (PlayerController.instance != null)
        {
            Destroy(PlayerController.instance.gameObject);
        }

        if (Managers.instance != null)
        {
            Destroy(Managers.instance.gameObject);
        }
        
        if (PersistentCanvas.instance != null)
        {
            Destroy(PersistentCanvas.instance.gameObject);
        }

        if (PersistentCamera.instance != null)
        {
            Destroy(PersistentCamera.instance.gameObject);
        }

        if (PauseMenu.instance != null)
        {
            Destroy(PauseMenu.instance.gameObject);
        }
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
