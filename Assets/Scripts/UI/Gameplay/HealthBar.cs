using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable playerDamageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. make sure it has the tag 'Player'");
            
            return;
        }

        playerDamageable = player.GetComponent<Damageable>();
    }

    void Start()
    {
        healthSlider.value = calculateSliderPercentage(playerDamageable.health, playerDamageable.maxHealth);
        healthBarText.text = "HP " + playerDamageable.health + "/" + playerDamageable.maxHealth;
    }
    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        newHealth = Mathf.Max(newHealth, 0);

        healthSlider.value = calculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP " + newHealth + "/" + maxHealth;
    }

    private float calculateSliderPercentage(float currentHealth, float maxHealth)
    {
        if (maxHealth == 0)
        {
            return 0f;
        }

        return currentHealth / maxHealth;
    }
}