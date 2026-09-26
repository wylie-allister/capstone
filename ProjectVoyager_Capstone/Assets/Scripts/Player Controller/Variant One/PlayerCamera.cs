using UnityEngine;
using UnityEngine.InputSystem;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxY = 90.0f;
    [SerializeField] private float minY = -90.0f;
    [SerializeField] private float controllerSensitivityScalar = 10.0f;

    private Vector3 _eulerAngles;
    public bool isGamepadActive = false;
    private float _currentSensitivity;
    
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        transform.eulerAngles = _eulerAngles = target.eulerAngles;

        _currentSensitivity = sensitivity;
    }

    public void UpdateRotation(CameraInput input)
    {

        // Update current sensitivity appropriately 
        _currentSensitivity = isGamepadActive ? sensitivity * controllerSensitivityScalar : sensitivity;
        
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * _currentSensitivity;

        // I think this is an alright way of doing this lmao
        // eulerangles scare me
        // this allows us to keep track of -180 to 180 degree motion
        // as euler angles only returns 0 to 360 (which is useless for clamping)
        float rotX = _eulerAngles.x;
        if (rotX > 180.0f)
        {
            rotX -= 360.0f;
        }

        _eulerAngles.x = Mathf.Clamp(rotX, minY, maxY);
        
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
}
