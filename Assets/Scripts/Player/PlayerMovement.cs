using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintMultiplier = 1f;
    [SerializeField] private float maxSprintCapacity = 5f;
    [SerializeField] private float crouchMultiplier = 1f;
    [SerializeField] private float originalHeight = 0.95f;

    [SerializeField] private InputReader inputReader;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 2.5f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    private Vector2 movementInput;
    private Rigidbody playerRigidbody;
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
    }

    private void OnDisable()
    {
        inputReader.OnPlayerMovement -= HandlePlayerMovement;
        inputReader.OnPlayerJump -= HandlePlayerJump;
        inputReader.OnPlayerSprint -= HandlePlayerSprint;
        inputReader.OnPlayerCrouch -= HandlePlayerCrouch;
        inputReader.OnPlayerLook -= HandlePlayerLook;
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
        if (IsGrounded())
        {
            // Remove existing vertical velocity
            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.y = 0f;
            playerRigidbody.linearVelocity = velocity;

            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandlePlayerSprint(bool isSprinting)
    {
        if(IsOnCrouch) return;

        sprintMultiplier = isSprinting ? 2f : 1f;

        IsOnSprint = isSprinting;
    }
    
    private void HandlePlayerCrouch(bool isCrouching)
    {   
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
    }

    void Update()
    {
        if(IsOnSprint && sprintCapacity > 0)
        {
            sprintCapacity -= Time.deltaTime;
        }
        else if(!IsOnSprint && sprintCapacity < maxSprintCapacity)
        {
            sprintCapacity += Time.deltaTime;
        }

        RotatePlayer();
    }

    private void FixedUpdate()
    {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * originalHeight);
    }
}
