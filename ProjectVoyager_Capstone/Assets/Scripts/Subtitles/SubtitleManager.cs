using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class SubtitleWrapper
{
    public Subtitle[] subtitles;
}

[Serializable]
public struct Subtitle
{
    public string subtitleText;
    public CharacterTag characterName;

    public Subtitle(string text, CharacterTag tag)
    {
        subtitleText = text;
        characterName = tag;
    }
}

public enum CharacterTag
{
    Cheddar,
    Cream,
    Cookie,
    Jackie,
    Bubba,
    Semmy
};

public class SubtitleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private Image letterboxImage;
    [SerializeField] private TextMeshProUGUI characterTagText;
    [SerializeField] private Image characterTagLetterboxImage;
    [SerializeField] private InputActionReference skipTextDisplay;
    [SerializeField] private GameObject subtitlePrefab;

    [Header("Current Subtitle")] 
    [SerializeField]
    private Subtitle currentSubtitle;

    [Header("Subtitle Settings")] 
    [Min(36)]
    [SerializeField] private float fontSize = 36;
    [SerializeField] private TMP_FontAsset font;
    [Range(1, 40)]
    [SerializeField] private int maxCharactersPerLine = 40;
    [Range(1, 3)]
    [SerializeField] private int maxLinesPerSubtitle = 2;
    [Range(0.1f, 200.0f)] 
    [SerializeField] private float textSpeed = 5.0f;

    [Header("Letterbox Settings")] 
    [Range(0.0f, 1.0f)]
    [SerializeField] private float letterboxOpacity = 0.9f;
    [SerializeField] private bool enableLetterbox = true;
    
    [Header("Existing Subtitles")] 
    public List<Subtitle> _existingSubtitles = new List<Subtitle>();

    private float _charDisplayTimer = 0.0f;
    private bool _displaySubtitle = false;
    private bool _displayLetterbox = false;
    private int _charsDisplayed = 0;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
    }

    public Subtitle CreateNewSubtitle(string newSubtitleText, CharacterTag characterName)
    {
        return new Subtitle(newSubtitleText, characterName);
    }

    public void UpdateCurrentSubtitle(Subtitle newSubtitle)
    {
        currentSubtitle = newSubtitle;
    }

    // Display current text via timer (display char's one at a time at a given speed)
    private void HandleTextDisplay(float deltaTime)
    {
    }

    // Display the subtitle (pop up subtitle call)
    private void DisplayCurrentSubtitle(float deltaTime)
    {
        _displaySubtitle = true;
        _displayLetterbox = enableLetterbox;
    }
    
    // Hide the subtitle
    private void HideCurrentSubtitle(float deltaTime)
    {
        _displaySubtitle = false;
        _displayLetterbox = false;
    }

    public void LoadExistingSubtitlesFromFile()
    {
        // Clear subs
        _existingSubtitles.Clear();
        
        // Get subtitles from subtitles file
        var subtitles = SaveSystem.Load<Subtitle>(JsonSavePath.Subtitles, "TestSubtitles");
        
        // append each subtitle from file to the current existing subtitle list
        foreach (var subtitle in subtitles)
            AddExistingSubtitle(subtitle);
        // Import subtitles here
    }

    private void AddExistingSubtitle(Subtitle newSubtitle)
    {
        _existingSubtitles.Add(newSubtitle);
    }
    
    public void SaveCurrentSubtitlesToFile()
    {
        SaveSystem.Save(_existingSubtitles, JsonSavePath.Options, "TestOptions");
    }

    public void DebugLogExistingSubtitles()
    {
        if (_existingSubtitles.Count > 0)
        {
            for (int i = 0; i < _existingSubtitles.Count; i++)
            {
                Debug.Log($"Subtitle {i+1} | {_existingSubtitles[i].characterName}: {_existingSubtitles[i].subtitleText}");
            }
        }
        else
        {
            Debug.Log("No subtitles found in Subtitle Manager.");
        }
    }

    public void SubtitlePopUp()//Subtitle subtitle)
    {
        // Toggle sub display
        _displaySubtitle = !_displaySubtitle;

        // If not displaying, break from function
        if (!_displaySubtitle)
            return;

        // TODO: REMOVE IMMEDIATELY, THIS IS ONLY FOR DEBUG TESTING
        Subtitle subtitle = _existingSubtitles[1];
        
        // Get subtitle info
        string subText = subtitle.subtitleText;
        string charTag = subtitle.characterName.ToString();

        // New string for formating 
        string popupText = string.Empty;

        for (int charIndex = 0; charIndex < subText.Length; charIndex++)
        {
            // Add char from subtitles text to the popup string
            popupText += subText[charIndex];
            
            // If at max characters, add the max char and add a new line
            if (charIndex > 2 && charIndex % maxCharactersPerLine == 1)
            {
                popupText += "\n";
            }
        }

        // Refresh subtitle settings to class settings
        RefreshSubtitleSettings();
        
        // Set subtitle text to the formatted string
        characterTagText.text = charTag;
        subtitleText.text = popupText;
        
        
    }

    private void RefreshSubtitleSettings()
    {
        // Set font and font size
        subtitleText.font = font;
        subtitleText.fontSize =  fontSize;
        
        characterTagText.font = font;
        characterTagText.fontSize = fontSize;
        
        // Get letterbox color with desired alpha
        Color desiredLetterboxColor = letterboxImage.color;
        desiredLetterboxColor.a = letterboxOpacity;
        
        // Set letterbox color to letterboxes
        letterboxImage.color = desiredLetterboxColor;
        characterTagLetterboxImage.color = desiredLetterboxColor;
        
        // Hide or Display letterbox when applicable
        letterboxImage.gameObject.SetActive(enableLetterbox);
    }


    // Look into custom editor window for saving subtitles directly then pulling
    // from save data to get the subtitle
}
