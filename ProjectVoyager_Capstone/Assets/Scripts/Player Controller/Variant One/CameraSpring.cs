using UnityEditor.SceneManagement;
using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    [SerializeField] private float halfLife = 0.065f;
    [SerializeField] private float frequency = 18.0f;
    [SerializeField] private float angularDisplacement = 2.0f;
    [SerializeField] private float linearDisplacement = 0.05f;
    private Vector3 _springPosition;
    private Vector3 _springVelocity;
    
    public void Initialize()
    {
        _springPosition = transform.position;
        _springVelocity = Vector3.zero;
    }

    public void UpdateSpring(float deltaTime, Vector3 up)
    {
        transform.localPosition = Vector3.zero;
        Spring(ref _springPosition, ref _springVelocity, transform.position, halfLife, frequency, deltaTime);
        
        var localSpringPos = _springPosition - transform.position;
        var springHeight = Vector3.Dot(localSpringPos, up);
        
        transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0.0f, 0.0f);
        transform.localPosition = localSpringPos * linearDisplacement;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _springPosition);
        Gizmos.DrawSphere(_springPosition, 0.1f);
    }

    // called every tick, use spring physics to affect velocity
    // freq = resonation
    private static void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfLife,
        float frequency, float timeStep)
    {
        var dampingRatio = -Mathf.Log(0.5f) / (frequency * halfLife);
        var f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
        var oo = frequency * frequency;
        var hoo = timeStep * oo;
        var hhoo = timeStep * hoo;
        var detInv = 1.0f / (f + hhoo);
        var detX = f * current + timeStep * velocity + hhoo * target;
        var detV = velocity + hoo * (target - current);
        current = detX * detInv;
        velocity = detV * detInv;
    }
}
