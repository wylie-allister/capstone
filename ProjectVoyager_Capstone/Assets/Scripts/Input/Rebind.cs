using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebind : MonoBehaviour
{
    public Button[] buttons;
    public TextMeshProUGUI[] labels;

    public InputActionAsset inputActions;
    public InputActionMap iMap;

    private InputActionRebindingExtensions.RebindingOperation rebind;

    private InputAction jump;
    private InputAction wallride;
    private InputAction slide;
    private InputAction dash;
    private InputAction interact;
    private InputAction swap;
    /*private InputAction move;
    private InputAction up;
    private InputAction down;
    private InputAction left;
    private InputAction right;*/
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jump = inputActions.FindAction("jump");
        wallride = inputActions.FindAction("wallride");
        slide = inputActions.FindAction("crouch");
        dash = inputActions.FindAction("dash");
        interact = inputActions.FindAction("interact");
        swap = inputActions.FindAction("swap");

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void LoadDefaultFromJSON()
    {
        inputActions.RemoveAllBindingOverrides();
    }

    public void Rebinds(int i)
    {
        
        inputActions.FindActionMap("Gameplay").Disable();
        buttons[i].interactable = false;
        if (i == 0)
        {
            rebind = jump.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }
        else if (i == 1)
        {

            rebind = wallride.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }
        else if (i == 2)
        {

            rebind = slide.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }
        else if (i == 3)
        {

            rebind = dash.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }
        else if (i == 4)
        {

            rebind = interact.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }
        else if (i == 5)
        {

            rebind = swap.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(i));
            rebind.Start();
        }

    }

    void RebindCompleted(int i)
    {
        rebind.Dispose();
        string newBinding = jump.bindings[i].effectivePath;
        labels[i].text = $"Currently: {newBinding}";
        buttons[i].interactable = true;
        inputActions.FindActionMap("Gameplay").Enable();

        var rebindss = inputActions.SaveBindingOverridesAsJson();
        //PlayerPrefs.SetString("rebinds", rebindss);
    }
}
