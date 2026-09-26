using UnityEngine;
using UnityEngine.InputSystem;

// Currently a singleton class, is currently utilized within the PlayerCharacter class within the OnGroundHit function
public class HapticController : MonoBehaviour
{
    private Gamepad _pad;
    public static HapticController instance;

    // SINGLETON AHAHA
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    
    void Start()
    {
        if (Gamepad.current != null)
        {
            _pad = Gamepad.current;
        }
        else
        {
            Debug.LogWarning("Gamepad not found in Haptic Controller script");
        }
    }

    public void QuickRumble()
    {
        if (_pad == null)
            return;
        
       StartRumble(0.3f, 0.15f, 0.1f);
    }
    
    public void LongRumble()
    {
        StartRumble(0.3f, 0.15f, 0.67f);
    }

    private void StartRumble(float lowFreq, float highFreq, float duration)
    {
        if (_pad != null)
        {
            // Low freq = left motor, High freq = right motor (Values: 0.0f to 1.0f)
            _pad.SetMotorSpeeds(lowFreq, highFreq);
            Invoke(nameof(StopRumble), duration);
        }
        
    }

    private void StopRumble()
    {
        if (_pad != null)
        {
            _pad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

}
