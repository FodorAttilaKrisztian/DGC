using UnityEngine;

public class Powerup : MonoBehaviour
{
    public PowerupEffect effect;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PowerupInventory inventory = FindFirstObjectByType<PowerupInventory>();

            if (inventory != null)
            {
                if(effect.name == "LifeBuff")
                {
                    bool effectApplied = effect.Apply(collision.gameObject);

                    if (effectApplied)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    inventory.StorePowerup(effect);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}