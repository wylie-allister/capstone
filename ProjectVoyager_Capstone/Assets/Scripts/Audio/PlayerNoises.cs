using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static UnityEditor.Profiling.RawFrameDataView;

public class PlayerNoises : MonoBehaviour
{
    
    private Player player;
    InputAction move;
    InputAction jump;
    InputAction dash;
    InputAction interact;

    public CompanionLog cl;

    FMOD.Studio.EventInstance playerState;
    private FMOD.Studio.EventInstance instance;

    public FMODUnity.EventReference bgMusic;
    public FMODUnity.EventReference footsteps;
    public FMODUnity.EventReference jumping;
    public FMODUnity.EventReference dashing;
    public FMODUnity.EventReference interacting;

    public int checkNum = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        dash = InputSystem.actions.FindAction("Dash");
        interact = InputSystem.actions.FindAction("Interact");
        playerState = FMODUnity.RuntimeManager.CreateInstance(footsteps);
        instance = FMODUnity.RuntimeManager.CreateInstance(bgMusic);
        instance.start();
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
        JankAssBGMusicChanger();
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

    public void Interact()
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
            // added grounded check for jumps just cause they can be spammed, needs to be fixed in fmod studio
            if (player.GetCharacterState().Grounded)
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

    public void JankAssBGMusicChanger()
    {
       

        instance.setParameterByName("Checkpoint", checkNum);
    }

    FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }
    }
