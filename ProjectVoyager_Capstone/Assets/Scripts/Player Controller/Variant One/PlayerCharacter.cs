using UnityEngine;
using KinematicCharacterController;

// Types of chrouch input
public enum CrouchInput
{
    None, Toggle, Crouch, UnCrouch
}

// Types of stances
public enum Stance
{
    Stand, Crouch, Slide
}

// Struct to hold character state variables
public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

// Struct that holds all relavent character input variables
public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
}

// Player character, Inherits from ICharacterController
public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    
    [Header("Movement Settings")]
    [Space] [SerializeField] private float walkSpeed = 20.0f;
    [SerializeField] private float crouchSpeed = 7.5f;
    [SerializeField] private float walkResponse = 25.0f;
    [SerializeField] private float crouchResponse = 20.0f;
    
    [Header("Air Movement Settings")]
    [Space] [SerializeField] private float airSpeed = 15.0f;
    [SerializeField] private float airAcceleration = 70.0f;  
    
    [Header("Jump and Gravity Settings")]
    [Space] [SerializeField] private float jumpSpeed = 20.0f;
    [SerializeField] private float coyoteTime = 0.2f;
    [Range(0.0f, 1.0f)] [SerializeField] private float jumpSustainGravity = 0.7f;
    [SerializeField] private float gravity = -90.0f;
    
    [Header("Slide Settings")]
    [Space] [SerializeField] private float slideStartSpeed = 25.0f;
    [SerializeField] private float slideEndSpeed = 15.0f;
    [SerializeField] private float slideFriction = 0.8f;
    [SerializeField] private float slideSteerAcceleration = 5.0f;
    [SerializeField] private float slideGravity = -90.0f;
    
    [Header("Height Settings")]
    [Space] [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchHeightResponse = 15.0f;
    
    [Header("Camera Height Settings")]
    [Range(0.1f, 1.0f)] [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0.1f, 1.0f)] [SerializeField] private float crouchCameraTargetHeight = 0.7f;


    private CharacterState _state;
    private CharacterState _tempState;
    private CharacterState _lastState;  
    
    

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedSustainedJump;
    private bool _requestedCrouch;
    private bool _requestedCrouchInAir;

    private float _timeSinceLastGround;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    private Collider[] _uncrouchOverlapResults;
    
    public void Initialize()
    {
        // Set state to standing
        _state.Stance = Stance.Stand;
        _lastState = _state;
        
        _uncrouchOverlapResults = new Collider[8];
        
        // Set character controller to this instance
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        // Get requested input
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0.0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1.0f);
        _requestedMovement = input.Rotation * _requestedMovement;
        
        // If we want to jump, set request jump var accordingly
        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        
        // Ground timer reset for coyote time
        if (_requestedJump && !wasRequestingJump)
        {
            _timeSinceJumpRequest = 0.0f;
        }
        
        _requestedSustainedJump = input.JumpSustain;
        
        var wasRequestingCrouch = _requestedCrouch;
        
        // Set requested crouch to val appropriate to crouch input types
        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            CrouchInput.Crouch => true,
            CrouchInput.UnCrouch => false
        };

        // if we havent crouched and want to, toggle based on air state
        if (_requestedCrouch && !wasRequestingCrouch)
        {
            _requestedCrouchInAir = !_state.Grounded;
        }
        else if (!_requestedCrouch && wasRequestingCrouch)
        {
            _requestedCrouchInAir = false;
        }
    }

    public void UpdateBody(float deltaTime)
    {
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;
        
        var cameraTargetHeight = currentHeight * (_state.Stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight);

        var rootTargetScale = new Vector3(1.0f, normalizedHeight, 1.0f);

        // t: a = lerp(a,b,1-exp(-delta*24.32))
        // ensure frame independence
        cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, new Vector3(0f, cameraTargetHeight, 0f), 
            1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
        
        root.localScale = Vector3.Lerp(root.localScale, rootTargetScale, 
            1f - Mathf.Exp(-crouchHeightResponse * deltaTime));

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, motor.CharacterUp);
        
        if (forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;
        // Slides only applicable on ground state 
        if (motor.GroundingStatus.IsStableOnGround)
        {
            // Reset last ground tracker for coyote time
            _timeSinceLastGround = 0.0f;
            _ungroundedDueToJump = false;
            // Snap requested movement dir to angle of surface char is on
            var groundedMovement = motor.GetDirectionTangentToSurface(direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal) * _requestedMovement.magnitude;

            
            // Start sliding
            {
                var moving = groundedMovement.sqrMagnitude > 0.0f;
                var crouching = _state.Stance is Stance.Crouch;
                var wasStanding = _lastState.Stance is Stance.Stand;
                var wasInAir = _lastState.Grounded;
                
                if (moving && crouching && (wasStanding || wasInAir))
                {
                    //Debug.DrawRay(transform.position, currentVelocity, Color.red, 5.0f);
                    //Debug.DrawRay(transform.position, _lastState.Velocity, Color.green, 5.0f);
                    _state.Stance = Stance.Slide;
                    
                    // if landing on stable ground, vel is projected onto a flat plane
                    // KinematicCharacterMotor.HandleVelocityProjection()
                    // 

                    if (wasInAir)
                    {
                        currentVelocity =
                            Vector3.ProjectOnPlane(_lastState.Velocity, motor.GroundingStatus.GroundNormal);
                    }


                    var effectiveSlideStartSpeed = slideStartSpeed;
                    if (!_lastState.Grounded && !_requestedCrouchInAir)
                    {
                        effectiveSlideStartSpeed = 0.0f;
                        _requestedCrouchInAir = false;
                    }
                    
                    var slideSpeed = Mathf.Max(slideStartSpeed, currentVelocity.magnitude);
                    currentVelocity = motor.GetDirectionTangentToSurface(
                        direction: currentVelocity,
                        surfaceNormal: motor.GroundingStatus.GroundNormal) * slideSpeed; 

                }
            }
            // Move
            
            if (_state.Stance is Stance.Stand or Stance.Crouch)
            {

                var speed = _state.Stance is Stance.Stand ? walkSpeed : crouchSpeed;

                var response = _state.Stance is Stance.Stand ? walkResponse : crouchResponse;
            
                // move along that dir
                var targetVelocity = _requestedMovement * speed;
                var moveVelocity = Vector3.Lerp(
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)); 
                _state.Acceleration = moveVelocity - currentVelocity;
                
                currentVelocity = moveVelocity;
            }
            // Sliding
            else
            {
                // Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);
                
                // Slope 
                {
                    var force = Vector3.ProjectOnPlane(-motor.CharacterUp, motor.GroundingStatus.GroundNormal) *
                                slideGravity;

                    currentVelocity -= force * deltaTime;
                }
                // Steering
                {
                    var currentSpeed = currentVelocity.magnitude;
                    var targetVelocity = groundedMovement * currentSpeed;
                    var steerVelocity = currentVelocity;
                    var steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                    // add steer force and clamp
                    steerVelocity += steerForce;
                    steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);
                    
                    _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;
                    currentVelocity = steerVelocity;
                }
                // stop
                if (currentVelocity.magnitude < slideEndSpeed)
                {
                    _state.Stance = Stance.Crouch;
                }
            }
        }
        // in air
        else
        {
            // increment coyote timer
            _timeSinceLastGround += deltaTime;
            
            // move
            if (_requestedMovement.sqrMagnitude > 0.0f)
            {
                // requested movement projected onto movement plane
                var planarMovement = Vector3.ProjectOnPlane(
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp) * _requestedMovement.magnitude;

                // current vel on move plane
                var currentPlanarVelocity = Vector3.ProjectOnPlane(
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp);

                // calc move force
                var movementForce = planarMovement * airAcceleration * deltaTime;

                if (currentPlanarVelocity.magnitude < airSpeed)
                {
                    // add to current pV for target vel
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    // limit targ vel to air speed
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                    
                    // steer to targ vel
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }
                // nerf movement force in the dir of curr planar vel
                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0.0f)
                {
                    var constrainedMovementForce =
                        Vector3.ProjectOnPlane(movementForce, currentPlanarVelocity.normalized);

                    movementForce = constrainedMovementForce;
                    
                }

                // Prevent air-climbing steep slopes
                if (motor.GroundingStatus.FoundAnyGround)
                {
                    // if moving in same dir as vel
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0.0f)
                    {
                        // Calculate obstruction normal
                        var obstructionNormal = Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)
                            .normalized;
                        
                        // Project movement force onto obstruction plane
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }
                
                // steer to current vel
                currentVelocity += movementForce;
            }
            
            var effectiveGravity = gravity;
            
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            
            if (_requestedSustainedJump && verticalSpeed > 0.0f)
            {
                effectiveGravity *= jumpSustainGravity;
            }
            
            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }

        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceLastGround < coyoteTime && !_ungroundedDueToJump;
            
            if (grounded || canCoyoteJump)
            {
                _requestedJump = false;         // Unset jump request
                _requestedCrouch = false;      // request uncrouch
                _requestedCrouchInAir = false;
                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;

                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequest +=  deltaTime;

                // buffer jump by coyote time
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                // deny jump - no jump for you
                _requestedJump = canJumpLater;
            }

        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempState = _state;
        
        // Crouch
        if (_requestedCrouch && _state.Stance is Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions(radius: motor.Capsule.radius, height: crouchHeight,
                yOffset: crouchHeight * 0.5f);
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
        {
            _state.Stance = Stance.Crouch;
        }
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Uncrouch
        if (!_requestedCrouch && _state.Stance is not Stance.Stand)
        {
            // "stand up"
            motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f);
            
            // check for problematic ceiling colls
            if (motor.CharacterOverlap(motor.TransientPosition, motor.TransientRotation, _uncrouchOverlapResults,
                    motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
            {
                _requestedCrouch = true;
                motor.SetCapsuleDimensions(radius: motor.Capsule.radius, height: crouchHeight,
                    yOffset: crouchHeight * 0.5f);
            }
            else
            {
                _state.Stance = Stance.Stand;
            }
        }

        // Update state to reflect relevant motor properties
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;
        
        // update last state to store char state snapshot at beginning of this update cycle
        _lastState = _tempState;
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
        Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
    
    public Transform GetCameraTarget() => cameraTarget;
    
    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;

    public void SetPosition(Vector3 position, bool killVelocity = true)
    {
        motor.SetPosition(position);
        if (killVelocity)
        {
            motor.BaseVelocity = Vector3.zero;
        }
    }
}
