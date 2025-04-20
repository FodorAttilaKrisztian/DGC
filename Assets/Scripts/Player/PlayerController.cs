using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]

public class PlayerController : MonoBehaviour, IDataPersistence
{
    public static PlayerController instance;
    private PlayerInput playerInput;
    private AudioManager audioManager;
    private UIManager uiManager;
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioManager = AudioManager.instance;
        playerInput = GetComponent<PlayerInput>();
        uiManager = FindFirstObjectByType<UIManager>();
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

        if (respawnPoint != Vector3.zero)
        {
            transform.position = respawnPoint;
        }
    }

    public void onPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PauseMenu.instance != null)
            {
                if (!PauseMenu.isPaused)
                {
                    PauseMenu.instance.PauseGame();
                }
            }
        }
    }

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
                if (isMoving && (!touchingDirections.IsOnWall))
                {
                    if(touchingDirections.IsGrounded)
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
        if (!PauseMenu.isPaused)
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
        if (!PauseMenu.isPaused)
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
    }

    public void onInteract(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            if (context.started && hasKey && isNearExit)
            {
                respawnPoint = Vector3.zero;
                DontDestroyOnLoad(gameObject);
                hasFireball = false;
                uiManager?.SetFireballUI(false);
                hasKey = false;
                uiManager?.SetKeyUI(false);
                uiManager.UpdatePowerupUI();
                DataPersistenceManager.instance.SaveGame();
                DataPersistenceManager.instance.GameData.uncollectedPowerups = new List<PowerupData>();
                SceneController.instance.NextLevel();
            }
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

    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (touchingDirections.IsGrounded && !touchingDirections.IsOnCeiling)
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
        if (!PauseMenu.isPaused)
        {
            if (context.started && canMove && isAlive)
            {
                if (touchingDirections.IsGrounded)
                {
                    animator.SetTrigger(AnimationStrings.jumpTrigger);
                    PlayJumpSound();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
                }
                else
                {
                    if (canDoubleJump)
                    {
                        animator.SetTrigger(AnimationStrings.jumpTrigger);
                        PlayJumpSound();
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
                        canDoubleJump = false;
                    }
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
        if (!PauseMenu.isPaused)
        {
            if (context.started)
            {
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }
        }   
    }

    public void onRangedAttack(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
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
                
                projectileLauncher.ProjectilePrefab = hasFireball ? fireballProjectilePrefab : defaultProjectilePrefab;
                
                rangedAttackCooldown = hasFireball ? fireballRangedCooldown : defaultRangedCooldown;
                lastRangedAttackTime = Time.time;
            }
        }
    }

    public void OnUsePowerup1(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            if (context.started)
            {
                powerupInventory.UsePowerup("HealthBuff");
            }
        }
    }

    public void OnUsePowerup2(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            if (context.started)
            {
                powerupInventory.UsePowerup("SpeedBuff");
            }
        }
    }

    public void OnUsePowerup3(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            if (context.started)
            {
                powerupInventory.UsePowerup("GravityBuff");
            }
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
            transform.position = respawnPoint;   
        }
    }

    public void RebindUIManager()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        uiManager?.SetKeyUI(hasKey);
        uiManager?.SetFireballUI(hasFireball);
        uiManager?.UpdatePowerupUI();
    }

    public void LoadData(GameData data)
    {
        damageable.health = data.currentHP; // Assuming damageable has a `currentHealth` property
        currentLives = data.currentLifeCount;
        transform.position = data.lastCheckpointPosition;
        hasFireball = data.fireballCollected;
        hasKey = data.keyCollected;
        powerupInventory.LoadData(data);
    }

    public void SaveData(ref GameData data)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        data.currentHP = damageable.health;
        data.currentLifeCount = currentLives;
        data.levelName = currentScene.name;
        data.lastCheckpointPosition = respawnPoint;
        data.fireballCollected = hasFireball;
        data.keyCollected = hasKey;
        powerupInventory.SaveData(ref data);
    }

    private void GameOver()
    {
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayHurtSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.PlayerHurtSound, 1f);
        }
    }

    public void PlayDeathSound()
    {
        if(audioManager != null)
        {
            audioManager.PlaySFX(audioManager.DeathSound, 0.75f);
        }
    }

    public void PlayGameOverSound()
    {
        if(audioManager != null)
        {
            audioManager.PlaySFX(audioManager.GameOverSound, 0.75f);
        }
    }

    public void PlayJumpSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.JumpSound, 0.1f);
        }
    }

    public void PlayPickupSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.PickupSound, 2f);
        }
    }

    public void PlayKeyPickupSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.KeyPickupSound, 2.5f);
        }
    }

    public void PlayRockThrowSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.RockThrowSound, 0.2f);
        }
    }

    public void PlayRockHitSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.RockHitSound, 0.1f);
        }
    }

    public void PlayFireballThrowSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.FireballThrowSound, 0.25f);
        }
    }

    public void PlayFireballHitSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.FireballHitSound, 0.15f);
        }
    }

    public void PlaySinglePunchSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.SinglePunchSound, 0.75f);
        }
    }

    public void PlayDoublePunchSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.DoublePunchSound, 0.75f);
        }
    }
}