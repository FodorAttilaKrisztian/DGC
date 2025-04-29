using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;

        [Header("Parallax Multipliers (0 = no movement, 1 = full camera movement)")]
        [Range(0f, 1f)] public float xMultiplier = 0.5f;
        [Range(0f, 1f)] public float yMultiplier = 0f;
    }

    [Header("Background Layers")]
    [SerializeField] private ParallaxLayer[] layers;

    private Transform cam;
    private Vector3 previousCamPos;

    private void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.position - previousCamPos;

        foreach (var layer in layers)
        {
            Vector3 move = new Vector3(delta.x * layer.xMultiplier, delta.y * layer.yMultiplier, 0f);
            layer.layerTransform.position += move;
        }

        previousCamPos = cam.position;
    }
}