using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] private Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        // Ensure animator is assigned
        if (transitionAnim == null)
        {
            transitionAnim = GetComponentInChildren<Animator>();
            if (transitionAnim == null)
            {
                Debug.LogError("SceneController: No Animator found! Transitions won't work.");
            }
        }
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelEndTrigger);
            yield return new WaitForSeconds(1);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        while (!operation.isDone)
        {
            yield return null; // Wait until the scene is fully loaded
        }

        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger(AnimationStrings.levelStartTrigger);
        }
    }
}