using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int lastScore = -1;

    void Update()
    {
        if (DataPersistenceManager.instance == null || DataPersistenceManager.instance.GameData == null)
        {
            Debug.Log("DataPersistenceManager or GameData is null. Cannot update score.");
            return;
        }

        int current = DataPersistenceManager.instance.GameData.score;

        if (current != lastScore)
        {
            lastScore = current;
            scoreText.text = $"Score: {current}";
        }
    }
}