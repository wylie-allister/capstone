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
   public static AccuracyManager instance;
   
   [SerializeField]
   private Player player;

   public double beatInterval;
   public double referenceDownbeatTime;
   public double timingWindow = 100;
   
   // SINGLETON AHAHA
   void Awake()
   {
      // Singleton instance pattern
      if (instance != null && instance != this)
      {
         Destroy(this.gameObject);
      }
      else
      {
         instance = this;
      }
   }
   
   void Start()
   {
      // Subscribe to beat delegates  
      AudioManagerRedux.onFixedBeat += OnDownBeat;
      AudioManagerRedux.onUpBeat += OnUpBeat;
   }

   void OnDestroy()
   {
      // Unsubscribe to beat delagates
      AudioManagerRedux.onFixedBeat -= OnDownBeat;
      AudioManagerRedux.onUpBeat -= OnUpBeat;
   }

   void Update()
   {
   }

   private void OnDownBeat()
   {
      // if nothings playing return
      if (!AudioManagerRedux.isPlayingMusic)
         return;
      
      // Else get the reference beat time as soon as the downbeat procs
      referenceDownbeatTime = AudioManagerRedux.GetCurrentTimeInMilliseconds();
      
      // Get beat interval * 1000 for ms
      beatInterval = AudioManagerRedux.GetBeatInterval() * 1000;
   }

   private void OnUpBeat()
   {
      
   }

   public Timing CheckInput(double inputTime)
   {
      // If not playing anything, return Timing.None
      if (!AudioManagerRedux.isPlayingMusic)
         return Timing.None;
      
      // If playing, get elapsed time and where the current beat is
      double elapsed = inputTime - referenceDownbeatTime;
      double beatPos = elapsed / beatInterval;

      // Find the nearest beat by rounding
      long nearestBeat = (long)Mathf.Round((float)beatPos);

      // Get target time
      double targBeatTime = referenceDownbeatTime + nearestBeat * beatInterval;

      // Get the difference between target time and when the player presses a key
      double diff = inputTime - targBeatTime;

      // Accuracy as the abs of diff
      double accuracy = Mathf.Abs((float)diff);

      /*
      Debug.Log(
         $"Target: {targBeatTime:F2} | " +
         $"Difference: {diff:F2} | " +
         $"Beat Index: {nearestBeat}" +
         $"Reference: {referenceDownbeatTime:F4}, " +
         $"Input: {inputTime:F4}"
      );*/

      // If accuracy is within 25, return perfect
      if (accuracy <= 25)
      {
         return Timing.Perfect;
      }

      // If accuracy is within 60, return great
      if (accuracy <= 60)
      {
         return Timing.Great;
      }

      // If accuracy is outside the timing window, return miss
      if (accuracy <= timingWindow)
         return Timing.Miss;
      
      // 
      return diff < 0 ? Timing.Early : Timing.Late;

   }

   public void OnInput()
   {
      double inputTiming = AudioManagerRedux.GetCurrentTimeInMilliseconds();

      var res = CheckInput(inputTiming);

      Debug.Log(res.ToString());
   }
   
}
