using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("The point from which the projectile will be fired.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("The projectile prefab to instantiate.")]
    [SerializeField] private GameObject projectilePrefab;

    public GameObject ProjectilePrefab
    {
        get => projectilePrefab;
        set => projectilePrefab = value;
    }

    public Transform FirePoint
    {
        get => firePoint;
        set => firePoint = value;
    }

    private void Reset()
    {
        if (firePoint == null) firePoint = transform;
    }

    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{nameof(ProjectileLauncher)}: Missing firePoint or projectilePrefab.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);
        
        Vector3 originalScale = projectile.transform.localScale;
        float facingDirection = Mathf.Sign(transform.localScale.x);
        projectile.transform.localScale = new Vector3(
            facingDirection * Mathf.Abs(originalScale.x),
            originalScale.y,
            originalScale.z
        );
    }
}