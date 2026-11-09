using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] 
    private Animator _armAnimator;
    
    [SerializeField] 
    private Animator _legAnimator;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            _armAnimator.SetTrigger("Debug");
        }
        
        #endif
    }

    public void TriggerSlide()
    {
        _armAnimator.SetTrigger("Sliding");
        _legAnimator.SetTrigger("Sliding");
    }
}
