using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PlayerNoises : MonoBehaviour
{
    InputAction move;

    FMOD.Studio.EventInstance playerState;
    public FMODUnity.EventReference footsteps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = InputSystem.actions.FindAction("Move");

    }

    // Update is called once per frame
    void Update()
    {


        if (move.WasPressedThisFrame())
        {
            FMODUnity.RuntimeManager.PlayOneShot(footsteps, transform.position);

        }
    }
}
