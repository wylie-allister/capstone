using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct InputTiming
{
    public double DashTiming;
    public double CrouchTiming;
    public double JumpTiming;
}

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
    public CharacterInput _exposedInput;
    public InputTiming InputTimings;
    
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

        // Initialize timings at default negative
        InputTimings = new InputTiming()
        {
            DashTiming = -1.0f,
            CrouchTiming = -1.0f,
            JumpTiming = -1.0f
        };

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
        
        
        PlayerInputActions.GameplayActions input = _inputActions.Gameplay;
        float deltaTime = Time.deltaTime;
        
        CameraInput cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);
        
        /*
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
        };*/

        CharacterInput characterInput = HandleCharacterInput(input, ref InputTimings);

        
        
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        CharacterState playerState = playerCharacter.GetState();

        playerCamera.WallRideLean(playerState.TouchingLeftWall, playerState.TouchingRightWall, playerState.Stance);
        
        if (input.Crouch.WasPressedThisFrame() && playerState.Stance != Stance.Slide)
        {
            playerAnim.TriggerSlide();
        }

        _exposedInput = characterInput;
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

    // Beat script ----------------------------------------------------------------------------------------------
    public bool CheckForBeatTiming(bool inputPressed, double inputTiming)
    {
        
        //var res = AccuracyManager.instance.CheckInput(inputTiming);
        
        //Debug.Log($"{res.ToString()} | {inputTiming}");
        return inputPressed;
    }

    public CharacterState GetCharacterState()
    {
        return playerCharacter.GetState();
    }

    private (bool wasActionPressed, double actionPressTiming) CheckInput(InputAction action)
    {
        bool input = action.WasPressedThisFrame();
        double timing = 0.0f;
        
        if (input) 
            timing = AudioManagerRedux.GetCurrentTimeInMilliseconds();
        
        
        
        return (input, timing);
    }

    private CharacterInput HandleCharacterInput(PlayerInputActions.GameplayActions input, ref InputTiming timings)
    {
        // Declare timestamp var
        double currentTimestamp = -1.0f;
        
        // Get inputs
        bool DashAction = input.Dash.WasPressedThisFrame();
        bool CrouchAction = input.Crouch.WasPressedThisFrame();
        bool JumpAction = input.Jump.WasPressedThisFrame();

        // Dont get timestamp if no actions were pressed
        if (DashAction || CrouchAction || JumpAction)
            if (AudioManagerRedux.isPlayingMusic)
                currentTimestamp = AudioManagerRedux.GetCurrentTimeInMilliseconds();

        // Set timings (this stops every action as counting as fired unless pressed
        // on the same frame)
        timings.DashTiming = DashAction ? currentTimestamp : -1.0f;
        timings.CrouchTiming = CrouchAction ? currentTimestamp : -1.0f;
        timings.JumpTiming = JumpAction ? currentTimestamp : -1.0f;

        // Create new charinput truct with alr checked inputs (removes double checks)
        CharacterInput tempInput = new CharacterInput()
        {
            Rotation     = playerCamera.transform.rotation,
            Move         = input.Move.ReadValue<Vector2>(),
            Dash         = DashAction,
            Jump         = JumpAction,
            JumpSustain  = input.Jump.IsPressed(),
            Crouch        = CrouchAction
                ? CrouchInput.Toggle
                : CrouchInput.None,
            WallRide = input.Wallride.IsPressed()
        };

        // return the inputs only
        return tempInput;
    }
}
