using UnityEngine;
using UnityEngine.InputSystem;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CurrentSettings settings; 
    [SerializeField] private float sensitivity;
    [SerializeField] private float maxY = 90.0f;
    [SerializeField] private float minY = -90.0f;
    [SerializeField] private float controllerSensitivityScalar = 10.0f;

    private Vector3 _eulerAngles;
    private float _currentSensitivity;
    public bool isGamepadActive = false;
    
    
    public void Initialize(Transform target)
    {
        // Set camera position and rotation to given camera target
        transform.position = target.position;
        transform.rotation = target.rotation;

        // Set transform + _eulerAngles var to given camera target euler angles
        transform.eulerAngles = _eulerAngles = target.eulerAngles;

        sensitivity = settings.sett.cameraSensitivity;
        // Set the current sensitivity to 
        _currentSensitivity = sensitivity;
    }

    public void UpdateRotation(CameraInput input)
    {
        //added these here as well to update sensitivity if changed in settings :)
        sensitivity = settings.sett.cameraSensitivity;
        _currentSensitivity = sensitivity;

        // If gamepad is being used, scale the sensitivity accordingly
        // This should probably be changed as I dont believe a direct increase in sensitivity is how this should
        // be handled but it works for right now :D -Brandon
        _currentSensitivity = isGamepadActive ? sensitivity * controllerSensitivityScalar : sensitivity;
        
        // Update euler angles with input
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * _currentSensitivity;

        // I think this is an alright way of doing this lmao
        // "The inner-machinations of eulerangles is an enigma" - Brandon
        // this allows us to keep track of -180 to 180 degree motion
        // as euler angles only returns 0 to 360 (which is useless for clamping)
        float currentRotationX = _eulerAngles.x;
        if (currentRotationX > 180.0f)
        {
            currentRotationX -= 360.0f;
        }

        // Update new clamped x val for eulerangles
        _eulerAngles.x = Mathf.Clamp(currentRotationX, minY, maxY);
        
        // Apply new eulerangles to transform
        transform.eulerAngles = _eulerAngles;
    }

    // Updates the transform position from the given target position
    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
}
