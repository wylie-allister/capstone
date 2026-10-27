using UnityEngine;
using UnityEngine.UI;

public class AccuracyManager : MonoBehaviour
{
    /* Currently not being used
    [SerializeField] private Image[] highlightedLetters;

    private bool _isTracking = false;
    private float _windowTimer = 0.0f;
    private float _windowTimerMax;
    private float _beatInterval;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideLetters();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTracking)
            Track();
    }

    void Track()
    {
        
    }
    
    void HighlightLetter(Image letterImage)
    {
        letterImage.color = new Color(letterImage.color.r, letterImage.color.g, letterImage.color.b, 1.0f);
    }

    void HideLetters()
    {
        foreach (Image image in highlightedLetters)
        {
            Color imgColor = image.color;

            image.color = new Color(imgColor.r, imgColor.g, imgColor.b, 0.0f);
        }
    }
    */
}
