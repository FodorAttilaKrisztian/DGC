using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Parallax : MonoBehaviour
{
    [Header("Scroll Speed (0 - 0.5)")]
    [Range(0f, 0.5f)]
    [SerializeField] private float speed = 0.2f;

    private Material mat;
    private float offset;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        offset += speed * Time.deltaTime;
        mat.SetTextureOffset("_MainTex", Vector2.right * offset);
    }
}