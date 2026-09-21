using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxY = 90.0f;
    [SerializeField] private float minY = -90.0f;
    private Vector3 _eulerAngles;
    
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;

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
