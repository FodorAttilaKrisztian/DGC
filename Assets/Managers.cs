using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}