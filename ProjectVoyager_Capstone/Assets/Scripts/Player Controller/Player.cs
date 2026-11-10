using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles input as well as Camera connections
/// </summary>
public class Player : MonoBehaviour
{
    // Script references
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [SerializeField] private PlayerAnimation playerAnim;

    [SerializeField] private Transform spawnLocation;
    [SerializeField] private bool disableMovement = false;
     // Input actions
    public PlayerInputActions _inputActions;
    public CurrentSettings cs;
    
    void Start()
    {
        GameObject settingsHandler = GameObject.Find("SettingsHandler");

        cs = settingsHandler.GetComponent<CurrentSettings>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        
        // Create and enable input actions
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        
        
        // Initialize character, camera, camera spring, and camera lean
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        
        cameraSpring.Initialize();
        cameraLean.Initialize();

        // Toggle gameplayactive var on camera to adjust joystick sens for camera
        playerCamera.isGamepadActive = Gamepad.current != null;
        
        // This throws an null ref - should automatically swap the sensitivity modifier in PlayerCamera.cs
        // Figure out why and fix it, cause this inside the update loop would work much better than one check at start
        //playerCamera.isGamepadActive = input.Look.activeControl.device is not Mouse;
    }

    // Destroy input actions on destroy call
    // Memory clean up :D
    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        if (cs.isOpen == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

        }

        if (disableMovement)
            return;
        
        
        var input = _inputActions.Gameplay;
        float deltaTime = Time.deltaTime;
        
        CameraInput cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        CharacterInput characterInput = new CharacterInput
        {
            Rotation     = playerCamera.transform.rotation,
            Move         = input.Move.ReadValue<Vector2>(),
            Dash         = input.Dash.WasPressedThisFrame(),
            Jump         = input.Jump.WasPressedThisFrame(),
            JumpSustain  = input.Jump.IsPressed(),
            Crouch        = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.Toggle
                : CrouchInput.None,
            WallRide = input.Wallride.IsPressed()
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        CharacterState playerState = playerCharacter.GetState();

        playerCamera.WallRideLean(playerState.TouchingLeftWall, playerState.TouchingRightWall, playerState.Stance);
        
        if (input.Crouch.WasPressedThisFrame() && playerState.Stance != Stance.Slide)
        {
            playerAnim.TriggerSlide();
        }
        
    }

    void LateUpdate()
    {
        if (disableMovement)
            return;
                
        // Get dt, camera target and current player state post updated variables
        float deltaTime = Time.deltaTime;
        Transform cameraTarget = playerCharacter.GetCameraTarget();
        CharacterState state = playerCharacter.GetState();
        
        // Update method calls with given params
        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
    }



    // Debug teleport method
    private void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

    private void UpdateSpawnPosition(Vector3 position)
    {
        playerCharacter.spawnLocation.position = position;
    }
    
}
