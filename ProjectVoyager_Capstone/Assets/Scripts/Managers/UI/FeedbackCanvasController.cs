using UnityEngine;
using UnityEngine.UI;

public class FeedbackCanvasController : MonoBehaviour
{
    // Order should be same as the enum
    // 0 - Early
    // 1 - Perfect
    // 2 - Great
    // 3 - Miss
    // 4 - Late
    [SerializeField]
    private Image[] beatFeedbackImages;

    private int currentFeedbackImage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
