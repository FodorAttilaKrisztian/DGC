using UnityEngine;
using TMPro;

public class EliminationCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eliminationText;
    private int lastEliminationsTotal = -1;

    void Update()
    {
        if (DataPersistenceManager.instance == null || DataPersistenceManager.instance.GameData == null)
        {
            return;
        }

        int current = DataPersistenceManager.instance.GameData.eliminationsTotal;

        if (current != lastEliminationsTotal)
        {
            lastEliminationsTotal = current;
            eliminationText.text = $"Skeletons eliminated: {current}";
        }
    }
}