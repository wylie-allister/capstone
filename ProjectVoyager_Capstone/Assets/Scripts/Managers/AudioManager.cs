using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private Image timingWindowViz;
    [SerializeField] private Image[] highlightedLetters;
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
    
    // Accuracy Tracker stuff
    private float _timingBuffer; // should be equal to the interval * 0.25 | interval / 4
    private bool _isTimingWindowOpen = false;
    private float DEBUGPLEASEDELETE = 0.0f;
    private bool[] wasdStates;
    private Color highlightColor = new Color(0.75f, 0.5f, 0.1f, 1.0f);
    private Color unhighlighttColor = new Color(0.75f, 0.5f, 0.1f, 0.0f);
    private int[] beatAccuracy;
    

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        metronomeCheck.localScale = Vector3.one * 0.2f;

        wasdStates = new bool[4];
        // beatAccuracy index 0 is made beats, 1 is missed beats
        beatAccuracy = new int[2];
        beatAccuracy[0] = 1;
        beatAccuracy[1] = 0;
    }

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        DebugInput(bpm, _deltaTime);
        TrackTimer(_deltaTime);
        
        //Debug.Log(_beatInterval * ((int)currentNoteDuration * 0.5f));

        // Lerp metronome scale down if in between beats
        if (metronomeCheck.localScale != Vector3.one * 0.2f)
        {
            metronomeCheck.localScale = Vector3.Lerp(metronomeCheck.localScale, Vector3.one * 0.2f, _deltaTime * 2.0f);
        }
    }

    // Debug input to begin tracking
    private void DebugInput(float trackBPM, float deltaTime)
    {
    #if UNITY_EDITOR
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            BeginBPMTrack(currentAudioClip, trackBPM);
        }
        
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentNoteDuration = NoteDuration.Quarter;
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentNoteDuration = NoteDuration.Half;
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentNoteDuration = NoteDuration.Whole;
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentNoteDuration = NoteDuration.Eighth;
        }

        wasdStates[0] = Keyboard.current.wKey.wasPressedThisFrame;
        wasdStates[1] = Keyboard.current.sKey.wasPressedThisFrame;
        wasdStates[2] = Keyboard.current.aKey.wasPressedThisFrame;
        wasdStates[3] = Keyboard.current.dKey.wasPressedThisFrame;
    #endif

        for (int i = 0; i < highlightedLetters.Length; i++)
        {
            highlightedLetters[i].color = wasdStates[i] ? highlightColor : unhighlighttColor;
        }
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
        
        
        _timingBuffer = 0.225f; //_beatInterval * 0.25f;
        
        
        _clipTimer = 0.0f;
        _previousNoteDuration = currentNoteDuration;
        _source.Play();
        _isTracking = true;
    }

    private void TrackTimer(float deltaTime)
    {
        // if tracking, update timer and continue
        if (_isTracking)
            _clipTimer += deltaTime;
        else
            return;

        // Open timing window when appropriate
        // |-------| refers to the space between two beats | == refers to the timing window being open
        // |==---==|
        if (_clipTimer >= _beatInterval * ((int)_previousNoteDuration * 0.5) - _timingBuffer ||
            _clipTimer <= _timingBuffer)
        {
            // Open timming window at the beginning of a beat, as well as near the end
            _isTimingWindowOpen = true;
        }
        else
        {
            _isTimingWindowOpen = false;
        }
        
        // if timer is greater than 
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

        timingWindowViz.color = _isTimingWindowOpen ? Color.green : Color.red;

        if (_isTimingWindowOpen)
        {
            DEBUGPLEASEDELETE += _deltaTime;
            
            // if timing window is open and the player presses a key, add it to hit beats
            foreach (var t in wasdStates)
            {
                if (t)
                {
                    beatAccuracy[0]++;
                }
            }
            timingWindowViz.color = Color.green;
        }
        else
        {
            // if timing window is closed and the player presses a key, add it to missed beats
            foreach (var t in wasdStates)
            {
                if (t)
                {
                    beatAccuracy[1]++;
                }
            }
            //Debug.Log($"tW Open for: {DEBUGPLEASEDELETE} | tB: {_timingBuffer}");
            DEBUGPLEASEDELETE = 0.0f;
            timingWindowViz.color = Color.red;
        }
        
    }

    // Logic block for when beats execute
    // Right now it just makes the circle visualizer jump in scale
    private void BeatExecute()
    {
        metronomeCheck.localScale = Vector3.one * 0.8f;
        // Correct * 100 / (correct + incorrect) provides a percentage of accuracy
        Debug.Log($"Accuracy: {((beatAccuracy[0] * 100) / (beatAccuracy[0] + beatAccuracy[1]))}% | {beatAccuracy[0]}, {beatAccuracy[1]}");
    }

}
