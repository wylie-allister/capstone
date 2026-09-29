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

    // Input actions
    private PlayerInputActions _inputActions;
    public CurrentSettings cs;
    
    void Start()
    {
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
        var input = _inputActions.Gameplay;
        float deltaTime = Time.deltaTime;

        if (cs.isOpen == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

        }

            CameraInput cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        CharacterInput characterInput = new CharacterInput
        {
            Rotation     = playerCamera.transform.rotation,
            Move         = input.Move.ReadValue<Vector2>(),
            Jump         = input.Jump.WasPressedThisFrame(),
            JumpSustain  = input.Jump.IsPressed(),
            Crouch        = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.Toggle
                : CrouchInput.None
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);
        
        // Teleport script for in editor purposes
        // tap T to teleport to wherever you are looking, assuming there is a ray collision (e.g. sky nono work)
        #if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            }
        }
        #endif
    }

    void LateUpdate()
    {
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
}
