using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]

public class PlayerController : MonoBehaviour, IDataPersistence
{
    private PowerupInventory powerupInventory;
    private Dictionary<string, Coroutine> activeBuffs = new Dictionary<string, Coroutine>();
    public PowerupEffect storedPowerup;
    private float originalGravityScale = 2.25f;
    public int maxLives = 3;
    public int currentLives; 
    private bool respawnTriggered = false;

    private float baseWalkSpeed;
    public float walkSpeed = 3f;

    private float baseRunSpeed;
    public float runSpeed = 6f;

    private float coyoteTime = 0.1f;

    private float coyoteTimeCounter = 0f;

    private float baseAirWalkSpeed;
    public float airWalkSpeed = 3f;

    private float baseAirRunSpeed;
    public float airRunSpeed = 6f;

    public float jumpImpulse = 10f;

    private bool canDoubleJump;

    Vector2 moveInput;
    
    TouchingDirections touchingDirections;

    Damageable damageable;

    public Vector3 respawnPoint;

    public float latestRespawnPointX;
    public float latestRespawnPointY;

    public bool hasKey = false;
    public bool isNearExit = false;

    public bool hasFireball = false;
    private ProjectileLauncher projectileLauncher;
    public GameObject defaultProjectilePrefab;
    public GameObject fireballProjectilePrefab;
    public float defaultRangedCooldown = 2f;
    public float fireballRangedCooldown = 1.25f;
    public float rangedAttackCooldown = 2f;
    private float lastRangedAttackTime = -999f;

    public bool canMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    
    [SerializeField]
    private bool _isMoving = false;
 
    public bool isMoving { 
        get { 
            return _isMoving;
        } 
        private set {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        } 
    }

    public float currentMoveSpeed { 
        get {
            if (canMove)
            {
                if (isMoving && (!touchingDirections.isOnWall))
                {
                    if(touchingDirections.isGrounded)
                    {
                        if (isRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        if (isRunning)
                        {
                            return airRunSpeed;
                        }
                        else
                        {
                            return airWalkSpeed;
                        }
                    }
                }
                else
                {
                    return 0;
                } 
            }
            else
            {
                return 0;
            }
        } 
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (isAlive)
        {
            isMoving = moveInput != Vector2.zero;

            setFacingDirection(moveInput);
        }
        else
        {
            isMoving = false;
        }
    }

    [SerializeField]
    private bool _isRunning = false;
 
    public bool isRunning { 
        get { 
            return _isRunning;
        } 
        private set {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        } 
    }

    public void onRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }
    public void onInteract(InputAction.CallbackContext context)
    {
        if (context.started && hasKey && isNearExit)
        {
            SceneController.instance.NextLevel();
        }
    }

    public bool _isFacingRight = true;
    public bool isFacingRight { 
        get { 
            return _isFacingRight;
        } 
        private set {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        } 
    }

    private void setFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
        }
    }

    public bool isAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }
 
    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        currentLives = maxLives;
        projectileLauncher = GetComponent<ProjectileLauncher>();
        powerupInventory = FindFirstObjectByType<PowerupInventory>();

        baseWalkSpeed = walkSpeed;
        baseRunSpeed = runSpeed;
        baseAirWalkSpeed = airWalkSpeed;
        baseAirRunSpeed = airRunSpeed;
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (touchingDirections.isGrounded && !touchingDirections.isOnCeiling)
            {   
                canDoubleJump = true;
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (!damageable.lockVelocity)
            {
                rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);
            }

            animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
        }
        else
        {
            if (!respawnTriggered)
            {
                RespawnSetup();
                Invoke("RespawnPlayer", 1.3f);
            }
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started && canMove && isAlive)
        {
            if (touchingDirections.isGrounded)
            {
                animator.SetTrigger(AnimationStrings.jumpTrigger);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
            }
            else
            {
                if (canDoubleJump)
                {
                    animator.SetTrigger(AnimationStrings.jumpTrigger);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
                    canDoubleJump = false;
                }
            }
        }
    }

    public void onHit(int damage, Vector2 knockBackForce)
    {
        rb.linearVelocity = new Vector2(knockBackForce.x, rb.linearVelocity.y + knockBackForce.y);
    }

    public void onAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void onRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= lastRangedAttackTime + rangedAttackCooldown)
        {
            if (hasFireball)
            {
                animator.SetTrigger(AnimationStrings.fireBallTrigger);
            }
            else
            {
                animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
            }
            
            projectileLauncher.projectilePrefab = hasFireball ? fireballProjectilePrefab : defaultProjectilePrefab;
            
            rangedAttackCooldown = hasFireball ? fireballRangedCooldown : defaultRangedCooldown;
            lastRangedAttackTime = Time.time;
        }
    }

    public void OnUsePowerup1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            powerupInventory.UsePowerup("HealthBuff");
        }
    }

    public void OnUsePowerup2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            powerupInventory.UsePowerup("SpeedBuff");
        }
    }

    public void OnUsePowerup3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            powerupInventory.UsePowerup("GravityBuff");
        }
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        if (activeBuffs.ContainsKey("SpeedBuff"))
        {
            StopCoroutine(activeBuffs["SpeedBuff"]);
            activeBuffs.Remove("SpeedBuff");

            ResetSpeedValues();
        }

        Coroutine buffCoroutine = StartCoroutine(SpeedBuffCoroutine(multiplier, duration));
        activeBuffs["SpeedBuff"] = buffCoroutine;
    }

    private IEnumerator SpeedBuffCoroutine(float multiplier, float duration)
    {
        walkSpeed *= multiplier;
        runSpeed *= multiplier;
        airWalkSpeed *= multiplier;
        airRunSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        ResetSpeedValues();
    }

    private void ResetSpeedValues()
    {
        walkSpeed = baseWalkSpeed; 
        runSpeed = baseRunSpeed;
        airWalkSpeed = baseAirWalkSpeed;
        airRunSpeed = baseAirRunSpeed;
    }

    public void ApplyGravityBuff(float gravityValue, float duration)
    {
        if (activeBuffs.ContainsKey("GravityBuff"))
        {
            StopCoroutine(activeBuffs["GravityBuff"]);
            activeBuffs.Remove("GravityBuff");

            ResetGravity();
        }

        Coroutine buffCoroutine = StartCoroutine(GravityBuffCoroutine(gravityValue, duration));
        activeBuffs["GravityBuff"] = buffCoroutine;
    }

    private IEnumerator GravityBuffCoroutine(float gravityValue, float duration)
    {
        rb.gravityScale = gravityValue;

        yield return new WaitForSeconds(duration);

        ResetGravity();
    }

    private void ResetGravity()
    {
        rb.gravityScale = originalGravityScale;
    }


    public void RespawnSetup()
    {
        respawnTriggered = true;

        if (respawnPoint.x > latestRespawnPointX)
        {
            latestRespawnPointX = respawnPoint.x;
            latestRespawnPointY = respawnPoint.y;
        }

        currentLives--;
    }

    public void RespawnPlayer()
    {
        respawnTriggered = false;

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            damageable.isAlive = true;
            animator.SetBool(AnimationStrings.isAlive, true);
            damageable.Heal(damageable.maxHealth);
            animator.SetBool(AnimationStrings.canMove, true);
            transform.position = new Vector3(latestRespawnPointX, latestRespawnPointY - 0.1f, 0);   
        }
    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Returning to Main Menu.");
        SceneManager.LoadScene("MainMenu");
    }
}