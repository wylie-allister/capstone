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
        // Singleton instance pattern
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
        // If the gamepad exists, set _pad accordingly or log error
        if (Gamepad.current != null)
        {
            _pad = Gamepad.current;
        }
        else
        {
            Debug.LogWarning("Gamepad not found in Haptic Controller script");
        }
    }

    /// <summary>
    /// Provides a quick haptic rumble | lF: 0.3f | hF: 0.15f | dur: 0.1f
    /// </summary>
    public void QuickRumble()
    {
        if (_pad == null)
            return;
        
       StartRumble(0.3f, 0.15f, 0.1f);
    }
    
    /// <summary>
    /// Provides a long haptic rumble | lF: 0.3f | hF: 0.15f | dur: 0.67f
    /// </summary>
    public void LongRumble()
    {
        StartRumble(0.3f, 0.15f, 0.67f);
    }

    /// <summary>
    /// Starts a haptic vibration with the given frequency params. Invokes StopRumble with given duration
    /// </summary>
    /// <param name="lowFreq"></param>
    /// <param name="highFreq"></param>
    /// <param name="duration"></param>
    private void StartRumble(float lowFreq, float highFreq, float duration)
    {
        if (_pad != null)
        {
            // Low freq = left motor, High freq = right motor (Values: 0.0f to 1.0f)
            _pad.SetMotorSpeeds(lowFreq, highFreq);
            Invoke(nameof(StopRumble), duration);
        }
        
    }

    /// <summary>
    /// Resets the gamepads motor speeds to 0. Effectively stopping any haptic vibration
    /// </summary>
    private void StopRumble()
    {
        if (_pad != null)
        {
            _pad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

}
