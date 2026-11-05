using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;


public class AudioManagerRedux : MonoBehaviour
{
    public static AudioManagerRedux instance;
    
    [SerializeField] 
    private EventReference music;

    public TimelineInfo timelineInfo = null;
    private GCHandle timelineHandle;
    
    private FMOD.Studio.EventInstance musicInstance;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }
    
    private void Awake()
    {
        instance = this;

        if (music.Path != null)
        {
            musicInstance = RuntimeManager.CreateInstance(music);
            musicInstance.start();
        }
        
    }

    private void Start()
    {
        if (music.Path != null)
        {
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
            musicInstance.setCallback(beatCallback,
                FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            
            
        }
    }

    private void OnDestroy()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
        timelineHandle.Free();
    }
    
    #if UNITY_EDITOR

    void OnGUI()
    {
        GUILayout.Box($"Current Beat = {timelineInfo.currentBeat}, Last Marker = {(string)timelineInfo.lastMarker}");
    }
    #endif
    
    // return type of fmod.result, which is an error code type
    // unmanaged memory yipeeee
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback Error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}


/*
public class AudioManagerRedux : MonoBehaviour
{
    private float currentBPM;
    private NoteDuration currentNoteDuration = NoteDuration.Quarter;

    private float _deltaTime;
    private float _trackTimer;
    private float _beatInterval;
    private bool _isTracking = false;
    private bool _canExecuteBeat = false;

    private float _timingBuffer;
    private bool _isTimingWindowOpen = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        HandleTrackTimer(_deltaTime);
    }

    private void HandleTrackTimer(float deltaTime, NoteDuration noteDuration)
    {
        if (_isTracking)
            _trackTimer += deltaTime;
        else
            return;

        if (_trackTimer >= _beatInterval * ((int)noteDuration * 0.5) - _timingBuffer ||
            _trackTimer <= _timingBuffer)
        {
            _isTimingWindowOpen = true;
        }
        else
        {
            _isTimingWindowOpen = false;
        }
        
    }
}
*/
