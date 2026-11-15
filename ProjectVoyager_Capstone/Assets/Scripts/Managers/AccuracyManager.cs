using UnityEngine;
using UnityEngine.UI;

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

      
      AudioManagerRedux.onFixedBeat += OnDownBeat;
      AudioManagerRedux.onUpBeat += OnUpBeat;
   }

   void OnDestroy()
   {
      AudioManagerRedux.onFixedBeat -= OnDownBeat;
      AudioManagerRedux.onUpBeat -= OnUpBeat;
   }

   void Update()
   {
   }

   private void OnDownBeat()
   {
      if (!AudioManagerRedux.isPlayingMusic)
         return;
      
      referenceDownbeatTime = AudioManagerRedux.GetCurrentTimeInMilliseconds();
      beatInterval = AudioManagerRedux.GetBeatInterval() * 1000;
   }

   private void OnUpBeat()
   {
      
   }

   public Timing CheckInput(double inputTime)
   {
      if (!AudioManagerRedux.isPlayingMusic)
         return Timing.None;
      double elapsed = inputTime - referenceDownbeatTime;
      double beatPos = elapsed / beatInterval;

      long nearestBeat = (long)Mathf.Round((float)beatPos);

      double targBeatTime = referenceDownbeatTime + nearestBeat * beatInterval;

      double diff = inputTime - targBeatTime;

      double accuracy = Mathf.Abs((float)diff);

      /*
      Debug.Log(
         $"Target: {targBeatTime:F2} | " +
         $"Difference: {diff:F2} | " +
         $"Beat Index: {nearestBeat}" +
         $"Reference: {referenceDownbeatTime:F4}, " +
         $"Input: {inputTime:F4}"
      );*/
      
      //Debug.Log($"INPUT TIME: {inputTime} | REFERENCE: {referenceDownbeatTime} | BEAT INTERVAL: {beatInterval}");
      
      if (accuracy <= 25)
      {
         return Timing.Perfect;
      }

      if (accuracy <= 60)
      {
         return Timing.Great;
      }

      if (accuracy <= timingWindow)
         return Timing.Miss;
      
      return diff < 0 ? Timing.Early : Timing.Late;

   }

   public void OnInput()
   {
      double inputTiming = AudioManagerRedux.GetCurrentTimeInMilliseconds();

      var res = CheckInput(inputTiming);

      Debug.Log(res.ToString());
   }
   
}
