using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;

    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;

        projectile.transform.localScale = new Vector3(
            (transform.localScale.x > 0 ? 1 : -1) * Mathf.Abs(origScale.x),
            origScale.y,
            origScale.z
        );
    }
}