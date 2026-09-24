using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static PlayerInput_Actions;


[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    #region Actions
    public event Action<Vector2> OnPlayerMovement;
    public event Action<bool> OnPlayerSprint;
    public event Action<bool> OnPlayerCrouch;
    public event Action<bool> OnPlayerAim;
    public event Action OnPlayerShoot;
    public event Action OnPlayerLook;
    public event Action OnPlayerJump;
    public event Action OnPlayerDash;
    public event Action OnPlayerExtract;
    #endregion

    #region Input Values
    public Vector2 lookInput;
    #endregion

    private PlayerInput_Actions playerInputActions;
    void OnEnable()
    {
        if (playerInputActions == null)
        {
            playerInputActions = new PlayerInput_Actions();
            playerInputActions.Player.SetCallbacks(this);
        }
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        OnPlayerAim?.Invoke(context.performed);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        OnPlayerCrouch?.Invoke(context.performed);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        OnPlayerDash?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        OnPlayerJump?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        OnPlayerLook?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnPlayerMovement?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        OnPlayerSprint?.Invoke(context.performed);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        OnPlayerShoot?.Invoke();
    }

    public void OnExtract(InputAction.CallbackContext context)
    {
        OnPlayerExtract?.Invoke();
    }
}
