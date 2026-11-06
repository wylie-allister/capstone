using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] 
    private Animator _animator;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerSlide()
    {
        _animator.SetTrigger("Dashing");
    }
}
