using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D), typeof(Animator))]
public class TouchingDirections : MonoBehaviour
{
    [Header("Detection Settings")]
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float ceilingDistance = 0.05f;
    public float wallDistance = 0.15f;

    private CapsuleCollider2D touchingCollider;
    private Animator animator;

    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnCeiling;
    [SerializeField] private bool _isOnWall;

    public bool IsGrounded
    {
        get => _isGrounded;
        set
        {
            _isGrounded = value;

            if (animator != null)
            {
                animator.SetBool(AnimationStrings.isGrounded, value);
            }
        }
    }

    public void SetGrounded(bool grounded)
    {
        IsGrounded = grounded;
    }

    public bool IsOnCeiling
    {
        get => _isOnCeiling;
        set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }

    public bool IsOnWall
    {
        get => _isOnWall;
        set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    private Vector2 WallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void Awake()
    {
        touchingCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 position = touchingCollider.bounds.center;
        Vector2 bottom = new(position.x, touchingCollider.bounds.min.y);
        Vector2 top = new(position.x, touchingCollider.bounds.max.y);

        float wallX = transform.localScale.x > 0 ? touchingCollider.bounds.max.x : touchingCollider.bounds.min.x;
        float halfHeight = touchingCollider.bounds.size.y / 2f;
        float sideOffset = halfHeight - 0.3f;

        Vector2[] wallCheckPoints =
        {
            new(wallX, position.y + halfHeight),
            new(wallX, position.y),
            new(wallX, position.y - halfHeight * 0.8f),
            new(wallX, position.y + sideOffset),
            new(wallX, position.y - sideOffset)
        };

        IsGrounded = IsAnyGroundRayHit(bottom);
        IsOnCeiling = Physics2D.Raycast(top, Vector2.up, ceilingDistance, castFilter.layerMask);
        IsOnWall = IsAnyWallRayHit(wallCheckPoints);

        DrawDebugRays(bottom, top, wallCheckPoints);
    }

    private bool IsAnyGroundRayHit(Vector2 bottom)
    {
        float halfWidth = touchingCollider.bounds.size.x / 4f;

        Vector2[] groundCheckPoints =
        {
            bottom,
            new(bottom.x - halfWidth, bottom.y),
            new(bottom.x + halfWidth, bottom.y)
        };

        foreach (var point in groundCheckPoints)
        {
            if (Physics2D.Raycast(point, Vector2.down, groundDistance, castFilter.layerMask))
                return true;
        }

        return false;
    }

    private bool IsAnyWallRayHit(Vector2[] points)
    {
        foreach (var point in points)
        {
            if (Physics2D.Raycast(point, WallCheckDirection, wallDistance, castFilter.layerMask))
                return true;
        }

        return false;
    }

    private void DrawDebugRays(Vector2 bottom, Vector2 top, Vector2[] wallCheckPoints)
    {
        Debug.DrawRay(bottom, Vector2.down * groundDistance, Color.green);
        Debug.DrawRay(new Vector2(bottom.x - touchingCollider.bounds.size.x / 4f, bottom.y), Vector2.down * groundDistance, Color.yellow);
        Debug.DrawRay(new Vector2(bottom.x + touchingCollider.bounds.size.x / 4f, bottom.y), Vector2.down * groundDistance, Color.yellow);

        Debug.DrawRay(top, Vector2.up * ceilingDistance, Color.red);

        Color[] wallColors = { Color.blue, Color.cyan, Color.blue, Color.magenta, Color.magenta };

        for (int i = 0; i < wallCheckPoints.Length; i++)
        {
            Debug.DrawRay(wallCheckPoints[i], WallCheckDirection * wallDistance, wallColors[i]);
        }
    }
}