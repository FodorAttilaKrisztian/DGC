using UnityEngine;
using TMPro;

public class BreakableCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI breakableText;
    private int lastBreakablesTotal = -1;

    void Update()
    {
        if (DataPersistenceManager.instance == null || DataPersistenceManager.instance.GameData == null)
        {
            return;
        }

        int current = DataPersistenceManager.instance.GameData.breakablesTotal;

        // Update only when value changes
        if (current != lastBreakablesTotal)
        {
            lastBreakablesTotal = current;
            breakableText.text = $"Breakables destroyed: {current}";
        }
    }
}