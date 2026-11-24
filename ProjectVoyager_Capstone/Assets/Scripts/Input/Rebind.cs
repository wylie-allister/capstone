using UnityEngine;
using UnityEngine.InputSystem;

public class Rebind : MonoBehaviour
{
    public InputActionReference dash;
    public InputAction interact;
    public Player player;
    public int bindingIndex;
    private InputActionRebindingExtensions.RebindingOperation rebind;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        if (interact.WasPerformedThisFrame())
        {
            
            RemapDash();
        }
    }

    void RemapDash()
    {
        var rbOp = dash.action.PerformInteractiveRebinding().WithTargetBinding(bindingIndex);
        rbOp.Start();
    }
}
