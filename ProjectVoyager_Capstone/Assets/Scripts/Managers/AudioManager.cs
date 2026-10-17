using UnityEngine;
using UnityEngine.InputSystem;

// Divide these when applying (allows for eight note 0.5 % division on int enums - bc
enum NoteDuration
{
    Eighth = 1,
    Quarter = 2,
    Half = 4,
    Whole = 8
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private RectTransform metronomeCheck;
    [SerializeField] private AudioClip currentAudioClip;
    [SerializeField] private float bpm;
    [SerializeField] private NoteDuration currentNoteDuration = NoteDuration.Quarter;

    private NoteDuration _previousNoteDuration;
    private AudioSource _source;
    private float _deltaTime;
    private float _clipLength;
    private float _clipTimer = 0.0f;
    private float _beatInterval;
    private float _beatBuffer = 0.05f;
    //private float _lastBeatTime;
    private bool _isTracking = false;
    private bool _canExecuteBeat = true;

    private int _numOfPastBeats = 0;
    

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        metronomeCheck.localScale = Vector3.one * 0.2f;
    }

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        DebugInput(bpm, _deltaTime);
        TrackTimer(_deltaTime);
        
        Debug.Log(_beatInterval * ((int)currentNoteDuration * 0.5f));

        HandleNoteDurationSelection();
        if (metronomeCheck.localScale != Vector3.one * 0.2f)
        {
            metronomeCheck.localScale = Vector3.Lerp(metronomeCheck.localScale, Vector3.one * 0.2f, _deltaTime * 2.0f);
        }
    }

    private void HandleNoteDurationSelection()
    {
        
    }

    private void DebugInput(float trackBPM, float deltaTime)
    {
    #if UNITY_EDITOR
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            BeginBPMTrack(currentAudioClip, trackBPM);
        }   
    #endif
    }

    private void BeginBPMTrack(AudioClip currentClip, float trackBPM)
    {
        // Break if the clip is playing
        if (_source.isPlaying) return;
        
        // Else set clip and clip info, Play clip
        _source.clip = currentClip;
        _source.volume = 0.75f;
        _source.loop = false;
        _numOfPastBeats = 0;
        _beatInterval = 60.0f / trackBPM;
        _clipTimer = 0.0f;
        _previousNoteDuration = currentNoteDuration;
        _source.Play();
        _isTracking = true;
    }

    private void TrackTimer(float deltaTime)
    {
        if (_isTracking)
            _clipTimer += deltaTime;
        else
            return;
        
        if (_clipTimer >= _beatInterval * ((int)_previousNoteDuration * 0.5f))
        {
            _canExecuteBeat = true;
            _clipTimer = 0.0f;
        }

        if (_canExecuteBeat)
        {
            BeatExecute();
            _previousNoteDuration = currentNoteDuration;
            _canExecuteBeat = false;
        }
        
    }

    // Temp log
    private void BeatExecute()
    {
        metronomeCheck.localScale = Vector3.one * 0.8f;
    }

    public void BeginTracking()
    {
        
    }

    public void StopTracking()
    {
        
    }
}
