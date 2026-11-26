using System;
using UnityEngine;
using UnityEngine.UI;

// Timing enum for accuracy values
public enum Timing
{
   Early,
   Perfect,
   Great,
   Miss,
   Late,
   None
}

public class AccuracyManager : MonoBehaviour
{

   private double beatTimestampMS = 0.0f;
   private double inputTimestampMS = 0.0f;
   private float inputWindowMS = 100.0f;
   private float playerInputDelay = 0.0f;


   private void Start()
   {
      // Subscribe methods to onbeat
      AudioManagerRedux.onFixedBeat += GetBeatTimestamp;
   }

   private void Update()
   {/*
      if (GameManager.instance.playerReference != null)
      {
         // Placeholder, only registers jump rn
         inputTimestampMS = GameManager.instance.playerReference.InputTimings.JumpTiming;
      }*/
   }

   private void OnDestroy()
   {
      // Unsubscribe methods
      AudioManagerRedux.onFixedBeat -= GetBeatTimestamp;
   }

   private void GetBeatTimestamp()
   {
      beatTimestampMS =  AudioManagerRedux.GetCurrentTimeInMilliseconds();
      playerInputDelay = (float)beatTimestampMS - (float)inputTimestampMS;
   }

}
