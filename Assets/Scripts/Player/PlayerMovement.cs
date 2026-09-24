using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintMultiplier = 1f;
    [SerializeField] private float maxSprintCapacity = 5f;
    [SerializeField] private Image sprintBarImage;
    [SerializeField] private float dashForce = 35;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchMultiplier = 1f;
    [SerializeField] private float originalHeight = 0.95f;

    [SerializeField] private InputReader inputReader;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 2.5f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    [Header("Health Settings")]
    [SerializeField] private float playerMaxHealth = 10;
    [SerializeField] private float playerHealth = 10;
    [SerializeField] private Image healthImageBar;

    
    [SerializeField] private float knockbackForce = 20;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachine;

    private Vector2 movementInput;
    private Rigidbody playerRigidbody;
    private bool isAlive = true;

    [Header("Debugging")]
    [SerializeField] private float sprintCapacity = 5;
    [SerializeField] private bool IsOnCrouch = false;
    [SerializeField] private bool IsOnSprint = false;

    [SerializeField] private float mouseSensitivity = 5f;

    private Vector2 lookInput;

    #endregion

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        playerRigidbody.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        inputReader.OnPlayerMovement += HandlePlayerMovement;
        inputReader.OnPlayerJump += HandlePlayerJump;
        inputReader.OnPlayerSprint += HandlePlayerSprint;
        inputReader.OnPlayerCrouch += HandlePlayerCrouch;
        inputReader.OnPlayerLook += HandlePlayerLook;
        inputReader.OnPlayerDash += HandlePlayerDash;
    }

    private void OnDisable()
    {
        inputReader.OnPlayerMovement -= HandlePlayerMovement;
        inputReader.OnPlayerJump -= HandlePlayerJump;
        inputReader.OnPlayerSprint -= HandlePlayerSprint;
        inputReader.OnPlayerCrouch -= HandlePlayerCrouch;
        inputReader.OnPlayerLook -= HandlePlayerLook;
        inputReader.OnPlayerDash -= HandlePlayerDash;
    }

    private void HandlePlayerLook()
    {
        lookInput = inputReader.lookInput;
    }
    private void HandlePlayerMovement(Vector2 movement)
    {
        movementInput = movement;
    }

    private void HandlePlayerJump()
    {
        if (IsGrounded() && isAlive)
        {
            // Remove existing vertical velocity
            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.y = 0f;
            playerRigidbody.linearVelocity = velocity;

            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandlePlayerDash()
    {
        if(!isAlive) return;

        if(IsOnCrouch) return;

        if(sprintCapacity < maxSprintCapacity/2) return;

        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();

        playerRigidbody.AddForce(direction * dashForce, ForceMode.Impulse);

        sprintCapacity -= maxSprintCapacity/2;
    }

    private void HandlePlayerSprint(bool isSprinting)
    {
        if(!isAlive) return;

        if(IsOnCrouch) return;

        if(IsOnSprint && !isSprinting)
        {
            if(sprintCapacity < maxSprintCapacity/4)
            {
                sprintMultiplier = isSprinting ? 2f : 1f;

                IsOnSprint = isSprinting;
            }
        }

        if(sprintCapacity < maxSprintCapacity/4) return;

        sprintMultiplier = isSprinting ? 2f : 1f;

        IsOnSprint = isSprinting;

    }
    
    private void HandlePlayerCrouch(bool isCrouching)
    {   
        if(!isAlive) return;
        
        if(isCrouching)
        {
            crouchMultiplier = 0.5f;
            sprintMultiplier = 1f;
            IsOnCrouch = true;
        }
        else
        {
            crouchMultiplier = 1f;
            IsOnCrouch = false;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        healthImageBar.fillAmount = playerHealth/playerMaxHealth;
    }

    void Update()
    {
        if(!isAlive) return;
        
        if(IsOnSprint && sprintCapacity > 0)
        {
            sprintCapacity -= Time.deltaTime;
        }
        else if(IsOnSprint && sprintCapacity <= 0)
        {
            IsOnSprint = false;
            sprintMultiplier = 1f;
        }
        else if(!IsOnSprint && sprintCapacity < maxSprintCapacity)
        {
            sprintCapacity += Time.deltaTime;
        }
        sprintBarImage.fillAmount = sprintCapacity/maxSprintCapacity;

        RotatePlayer();
    }

    private void FixedUpdate()
    {
        if(!isAlive) return;
        
        Vector3 moveDirection =
            (transform.forward * movementInput.y) +
            (transform.right * movementInput.x);

        moveDirection *= moveSpeed * sprintMultiplier * crouchMultiplier;

        playerRigidbody.linearVelocity = new Vector3(
            moveDirection.x,
            playerRigidbody.linearVelocity.y,
            moveDirection.z
        );

        ApplyJumpGravity();

        playerRigidbody.angularVelocity = Vector3.zero;
    }

    private void RotatePlayer()
    {
        float mouseX = lookInput.x;

        Quaternion rotation = Quaternion.Euler(
            0f,
            mouseX * mouseSensitivity * Time.deltaTime,
            0f
        );

        playerRigidbody.MoveRotation(
            playerRigidbody.rotation * rotation
        );
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, originalHeight);
    }

    private void ApplyJumpGravity()
    {
        if (playerRigidbody.linearVelocity.y < 0f)
        {
            // Falling
            playerRigidbody.AddForce(
                Physics.gravity * (fallGravityMultiplier - 1f),
                ForceMode.Acceleration
            );
        }
    }

    public void TakeDamage(float amount, Vector3 position)
    {
        playerHealth -= amount;

        healthImageBar.fillAmount = playerHealth/playerMaxHealth;

        if(playerHealth <= 0)
        {
            isAlive = false;
            GamePlayManager.Instance.GameEndedByDeath();
        }

        ApplyKnockback(position);
    }

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        Vector3 direction = transform.position - attackerPosition;
        direction.y = 0f;
        direction.Normalize();

        playerRigidbody.linearVelocity = Vector3.zero;

        playerRigidbody.AddForce(direction * knockbackForce, ForceMode.Impulse);

        StartCoroutine(GiveCameraShakeEffect());
    }

    private IEnumerator GiveCameraShakeEffect()
    {
        cinemachine.AmplitudeGain = 1;
        yield return new WaitForSeconds(0.2f);
        cinemachine.AmplitudeGain = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * originalHeight);
    }
}
