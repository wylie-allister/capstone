using UnityEngine;
using UnityEngine.InputSystem;

public class HapticTest : MonoBehaviour
{
    
    private bool _isRumbleActive = false;
    public float lowFreq, highFreq = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToggleRumble()
    {
        _isRumbleActive = !_isRumbleActive;
        
        if (_isRumbleActive)
        {
            Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);
        }
    }
    
    
}
