using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public static bool isPaused;

    private PlayerInput playerInput;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void onUnpause(InputAction.CallbackContext context)
    {
        if (context.started && isPaused)
        {
            ResumeGame();
        }
    }

    public void Start() 
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        if (PlayerController.instance != null)
        {
            playerInput = PlayerController.instance.GetComponent<PlayerInput>();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        PlayerController.instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;

        PlayerController.instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PlayerController.instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        isPaused = false;
        PlayerController.instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        DataPersistenceManager.instance.SaveGame();

        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
        #endif

        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE)
            Application.Quit();
        #elif (UNITY_WEBGL)
            SceneManager.LoadScene("QuitScene");
        #endif
    }
}
