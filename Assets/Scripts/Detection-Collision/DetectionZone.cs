using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DetectionZone : MonoBehaviour
{
    [Header("Detection Events")]
    [Tooltip("Invoked when no colliders remain in the detection zone.")]
    public UnityEvent noCollidersRemain;

    private readonly List<Collider2D> detectedColliders = new();
    private Collider2D detectionCollider;

    public IReadOnlyList<Collider2D> DetectedColliders => detectedColliders;

    private void Awake()
    {
        detectionCollider = GetComponent<Collider2D>();
        if (!detectionCollider.isTrigger)
        {
            Debug.LogWarning($"{nameof(DetectionZone)} requires the Collider2D to be set as a trigger.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!detectedColliders.Contains(collision))
        {
            detectedColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (detectedColliders.Remove(collision) && detectedColliders.Count == 0)
        {
            noCollidersRemain?.Invoke();
        }
    }
}