using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Skeleton : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    public LayerMask playerLayer;
    private Transform player;
    public float chaseRadius = 4f;
    private bool chasingPlayer = false; 

    public enum WalkableDirection { Right, Left };
    private WalkableDirection _walkDirection;

    public float walkAcceleration = 30f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;

    private Vector2 walkDirectionVector = Vector2.right;
    public bool canMove => animator.GetBool(AnimationStrings.canMove);

    public WalkableDirection walkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private void FlipDirection()
    {
        walkDirection = walkDirection == WalkableDirection.Right ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void onCliffDetected()
    {
        if(touchingDirections.isGrounded)
        {
            FlipDirection();
        }
    }

    public void onHit(int damage, Vector2 knockBackForce)
    {
        rb.linearVelocity = new Vector2(knockBackForce.x, rb.linearVelocity.y + knockBackForce.y);
    }

    public bool hasTarget
    {
        get => attackZone.detectedColliders.Count > 0;
        private set => animator.SetBool(AnimationStrings.hasTarget, value);
    }

    public float attackCooldown
    {
        get => animator.GetFloat(AnimationStrings.attackCooldown);
        private set => animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Update()
    {
        hasTarget = attackZone.detectedColliders.Count > 0;
        attackCooldown -= Time.deltaTime;

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, chaseRadius, playerLayer);
        
        if (playerCollider != null)
        {
            player = playerCollider.transform;
            chasingPlayer = true;
            maxSpeed = 4f;

        }
        else
        {
            player = null;
            chasingPlayer = false;
            maxSpeed = 3f;
        }
    }

    private void FixedUpdate()
    {
        bool nearCliff = cliffDetectionZone.detectedColliders.Count == 0;

        if (touchingDirections.isGrounded && (touchingDirections.isOnWall || nearCliff) && !chasingPlayer)
        {
            FlipDirection();
        }

        if (!damageable.lockVelocity)
        {
            if (chasingPlayer && player != null)
            {
                float directionToPlayer = player.position.x - transform.position.x;
                walkDirection = directionToPlayer > 0 ? WalkableDirection.Right : WalkableDirection.Left;
            }

            if (nearCliff)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            if (canMove && touchingDirections.isGrounded)
            {
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}