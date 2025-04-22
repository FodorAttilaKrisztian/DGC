using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class ProjectilePlayModeTests
{
    private GameObject projectileGO;
    private GameObject damageableGO;
    private Projectile projectile;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create Projectile GameObject
        projectileGO = new GameObject("Projectile");
        projectileGO.transform.position = Vector2.zero;
        var rb = projectileGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        projectile = projectileGO.AddComponent<Projectile>();
        projectileGO.AddComponent<CircleCollider2D>().isTrigger = true;

        // Set layer to Default
        projectileGO.layer = LayerMask.NameToLayer("Default");

        // Create Damageable GameObject
        damageableGO = new GameObject("Damageable");
        damageableGO.transform.position = new Vector2(5f, 0f);
        var collider = damageableGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        damageableGO.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        damageableGO.layer = LayerMask.NameToLayer("Default");

        damageableGO.AddComponent<MockDamageable>();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(projectileGO);
        Object.Destroy(damageableGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Projectile_HitsDamageable_And_DestroysItself()
    {
        var mock = damageableGO.GetComponent<MockDamageable>();
        projectileGO.transform.localScale = Vector3.one; // face right
        projectileGO.transform.position = new Vector3(-5f, 0f); // start left of target

        yield return new WaitForSeconds(0.1f); // Let velocity apply

        // Wait until projectile reaches target
        float timeout = 3f;
        while (projectileGO != null && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(mock.WasHit, "Projectile should have hit the mock Damageable.");
        Assert.IsTrue(projectileGO == null, "Projectile should have destroyed itself.");
    }

    // Simple mock for Damageable
    private class MockDamageable : Damageable
    {
        public bool WasHit = false;

        public override bool Hit(int damage, Vector2 knockBackForce)
        {
            WasHit = true;
            return true;
        }
    }
}