using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [SerializeField] private float attackDamping = 0.5f;
    [SerializeField] private float decayDamping = 0.3f;
    [SerializeField] private float walkStrength = 0.075f;
    [SerializeField] private float slideStrength = 0.025f;
    [SerializeField] private float strengthResponse = 5.0f;
    
    private Vector3 _dampedAcceleration;
    private Vector3 _dampedAccelerationVel;

    private float _smoothStrength;
    
    public void Initialize()
    {
        _smoothStrength = walkStrength;
    }

    public void UpdateLean(float deltaTime, bool sliding, Vector3 acceleration, Vector3 up)
    {
        var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude ? attackDamping : decayDamping;

        _dampedAcceleration = Vector3.SmoothDamp(_dampedAcceleration, planarAcceleration, ref _dampedAccelerationVel,
            damping, float.PositiveInfinity, deltaTime);
        
        // get rot axis from accel-vector
        var leanAxis = Vector3.Cross(_dampedAcceleration.normalized, up).normalized;
        
        // reset rot to parent
        transform.localRotation = Quaternion.identity;
        
        // Rotate around lean axis
        var effectiveStrength = sliding ? slideStrength : walkStrength;
        _smoothStrength = Mathf.Lerp(_smoothStrength, effectiveStrength, 1.0f - Mathf.Exp(-strengthResponse * deltaTime));
        transform.rotation = Quaternion.AngleAxis(_dampedAcceleration.magnitude * _smoothStrength, leanAxis) * transform.rotation;
    }
}
