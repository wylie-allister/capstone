using UnityEngine;

public class DebugBeatTicker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManagerRedux.onFixedBeat +=  DebugLogTest;
    }

    void OnDestroy()
    {
        AudioManagerRedux.onFixedBeat -= DebugLogTest;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void DebugLogTest()
    {
        Debug.Log("TEST BEAT HIT");
    }
}
