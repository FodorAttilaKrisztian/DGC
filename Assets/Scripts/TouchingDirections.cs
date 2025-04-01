using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    Animator animator;
    CapsuleCollider2D touchingCol;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    public ContactFilter2D castFilter;

    public float groundDistance = 0.05f; 
    public float ceilingDistance = 0.05f;
    public float wallDistance = 0.15f;

    [SerializeField]
    private bool _isGrounded;

    public bool isGrounded { get {
        return _isGrounded;
    } private set {
        _isGrounded = value;

        animator.SetBool(AnimationStrings.isGrounded, value);
    } }

    [SerializeField]
    private bool _isOnCeiling;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool isOnCeiling { get {
        return _isOnCeiling;
    } private set {
        _isOnCeiling = value;

        animator.SetBool(AnimationStrings.isOnCeiling, value);
    } }

    [SerializeField]
    private bool _isOnWall;

    public bool isOnWall { get {
        return _isOnWall;
    } private set {
        _isOnWall = value;

        animator.SetBool(AnimationStrings.isOnWall, value);
    } }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Vector2 position = touchingCol.bounds.center;
        Vector2 bottom = new Vector2(position.x, touchingCol.bounds.min.y);
        Vector2 top = new Vector2(position.x, touchingCol.bounds.max.y);

        float wallX = gameObject.transform.localScale.x > 0 ? touchingCol.bounds.max.x : touchingCol.bounds.min.x;

        Vector2 wallSideTop = new Vector2(wallX, position.y + (touchingCol.bounds.size.y / 2));
        Vector2 wallSideMiddle = new Vector2(wallX, position.y); 
        Vector2 wallSideBottom = new Vector2(wallX, position.y - (touchingCol.bounds.size.y / 2 * 0.8f));

        Vector2 wallSideHigh = new Vector2(wallX, position.y + (touchingCol.bounds.size.y / 2 - 0.3f)); 
        Vector2 wallSideLow = new Vector2(wallX, position.y - (touchingCol.bounds.size.y / 2 - 0.3f)); 

        isGrounded = Physics2D.Raycast(bottom, Vector2.down, groundDistance, castFilter.layerMask);

        RaycastHit2D groundCenter = Physics2D.Raycast(bottom, Vector2.down, groundDistance, castFilter.layerMask);
        RaycastHit2D groundLeft = Physics2D.Raycast(new Vector2(bottom.x - touchingCol.bounds.size.x / 4f, bottom.y), Vector2.down, groundDistance, castFilter.layerMask);
        RaycastHit2D groundRight = Physics2D.Raycast(new Vector2(bottom.x + touchingCol.bounds.size.x / 4f, bottom.y), Vector2.down, groundDistance, castFilter.layerMask);

        isGrounded = groundCenter.collider != null || groundLeft.collider != null || groundRight.collider != null;

        isOnCeiling = Physics2D.Raycast(top, Vector2.up, ceilingDistance, castFilter.layerMask);

        if (Physics2D.Raycast(wallSideTop, wallCheckDirection, wallDistance, castFilter.layerMask) ||
            Physics2D.Raycast(wallSideMiddle, wallCheckDirection, wallDistance, castFilter.layerMask) || 
            Physics2D.Raycast(wallSideBottom, wallCheckDirection, wallDistance, castFilter.layerMask) ||
            Physics2D.Raycast(wallSideHigh, wallCheckDirection, wallDistance, castFilter.layerMask) || 
            Physics2D.Raycast(wallSideLow, wallCheckDirection, wallDistance, castFilter.layerMask)) 
        {
            isOnWall = true;
        }
        else
        {
            isOnWall = false;
        }

        Debug.DrawRay(bottom, Vector2.down * groundDistance, Color.green);
        Debug.DrawRay(top, Vector2.up * ceilingDistance, Color.red);
        Debug.DrawRay(wallSideTop, wallCheckDirection * wallDistance, Color.blue);
        Debug.DrawRay(wallSideMiddle, wallCheckDirection * wallDistance, Color.cyan); // Middle debug ray
        Debug.DrawRay(wallSideBottom, wallCheckDirection * wallDistance, Color.blue);
        Debug.DrawRay(wallSideHigh, wallCheckDirection * wallDistance, Color.magenta); // High side debug ray
        Debug.DrawRay(wallSideLow, wallCheckDirection * wallDistance, Color.magenta); // Low side debug ray
        Debug.DrawRay(new Vector2(bottom.x - touchingCol.bounds.size.x / 4f, bottom.y), Vector2.down * groundDistance, Color.yellow);
        Debug.DrawRay(new Vector2(bottom.x + touchingCol.bounds.size.x / 4f, bottom.y), Vector2.down * groundDistance, Color.yellow);
    }
}