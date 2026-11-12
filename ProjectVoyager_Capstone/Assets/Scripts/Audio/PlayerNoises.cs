using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static UnityEditor.Profiling.RawFrameDataView;

public class PlayerNoises : MonoBehaviour
{
    InputAction move;
    InputAction jump;
    InputAction dash;

    FMOD.Studio.EventInstance playerState;
    public FMODUnity.EventReference footsteps;
    public FMODUnity.EventReference jumping;
    public FMODUnity.EventReference dashing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        dash = InputSystem.actions.FindAction("Dash");
        playerState = FMODUnity.RuntimeManager.CreateInstance(footsteps);
    }
    void OnDestroy()
    {
        playerState.release();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Jump();
        Dash();
    }

    public void Dash()
    {
        if (dash.WasPressedThisFrame())
        {
            FMODUnity.RuntimeManager.PlayOneShot(dashing, transform.position);
        }
        else if (dash.WasReleasedThisFrame())
        {
            playerState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            playerState.release();
        }
    }
    public void Jump()
    {
        if (jump.WasPressedThisFrame())
        {
            FMODUnity.RuntimeManager.PlayOneShot(jumping, transform.position);
        }
        else if (jump.WasReleasedThisFrame())
        {
            playerState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            playerState.release();
        }
    }
    public void Walk()
    {
        playerState.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        if (move.WasPressedThisFrame())
        {
            if (PlaybackState(playerState) != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                playerState = FMODUnity.RuntimeManager.CreateInstance(footsteps);
                playerState.start();
            }


        }
        else if (move.WasReleasedThisFrame())
        {
            if (PlaybackState(playerState) == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                playerState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                playerState.release();
            }

        }
    }

    FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }
    }
