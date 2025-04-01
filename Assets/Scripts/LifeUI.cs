using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    PlayerController playerController;

    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has the tag 'Player'");
            
            return;
        }

        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (playerController == null)
        {
            return;
        }

        int currentLives = playerController.currentLives;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}