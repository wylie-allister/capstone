using UnityEngine;

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
        
    }

    public void TriggerSlide()
    {
        _armAnimator.SetTrigger("Sliding");
        _legAnimator.SetTrigger("Sliding");
    }
}
