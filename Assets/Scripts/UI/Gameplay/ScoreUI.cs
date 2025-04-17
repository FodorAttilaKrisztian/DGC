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
            return;
        }

        int current = DataPersistenceManager.instance.GameData.score;

        // Update only when value changes
        if (current != lastScore)
        {
            lastScore = current;
            scoreText.text = $"Score: {current}";
        }
    }
}