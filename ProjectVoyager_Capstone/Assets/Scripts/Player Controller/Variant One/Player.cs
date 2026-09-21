using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;

    private PlayerInputActions _inputActions;
    
    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        
        // Get and enable input actions
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        
        // Initialize character, camera, camera spring, and camera lean
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        
        cameraSpring.Initialize();
        cameraLean.Initialize();
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
        var deltaTime = Time.deltaTime;

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput
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
        // tap T to teleport to wherever you are looking, assuming there is a
        #if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            }
        }
        #endif
    }

    void LateUpdate()
    {
        // Get dt, camera target and current player state
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();
        
        // Update method calls with given params
        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
    }

    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }
}
