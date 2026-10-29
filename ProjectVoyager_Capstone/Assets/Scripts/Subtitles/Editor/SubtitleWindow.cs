using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(SubtitleManager))]
public class SubtitleWindow : EditorWindow
{
    private string newSubtitleText = " ";
    private CharacterTag newCharacterTag;
    
    private SubtitleManager _subtitleManagerTarget;
    private SerializedObject _serializedSubtitleMTarget;
    private SerializedProperty _serializedSubtitleProperty;

    private GUIStyle wrappingTextStyle;

    [MenuItem("Voyager/Subtitle Editor")]
    public static void ShowWindow()
    {
        GetWindow<SubtitleWindow>("Voyager Subtitle Editor");
    }
    
    void OnGUI()
    {
        
        
        GUILayout.Label("Subtitle Editor", EditorStyles.boldLabel);

        CreateGUIStyles();
        newSubtitleText = (string)EditorGUILayout.TextField("New Subtitle Text", newSubtitleText, wrappingTextStyle, GUILayout.MinHeight(100));
        newCharacterTag = (CharacterTag)EditorGUILayout.EnumPopup("Character Tag", newCharacterTag);
        
        if (GUILayout.Button("Create New Subtitle"))
        {
            if (newSubtitleText == "")
            {
                Debug.LogWarning("Please enter new subtitle text");
                return;
            }
            
            HandleSubtitleCreation(newSubtitleText, newCharacterTag);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        HandleSerializedSubtitles();

        if (GUILayout.Button("Save Existing Subtitles"))
        {
            _subtitleManagerTarget.SaveCurrentSubtitlesToFile();
        }
        
        if (GUILayout.Button("Load Existing Subtitles"))
        {
            _subtitleManagerTarget.LoadExistingSubtitlesFromFile();
        }
        
        if (GUILayout.Button("Debug Log Subtitle Manager Subtitles"))
        {
            _subtitleManagerTarget.DebugLogExistingSubtitles();
        }
        
        if (GUI.changed)
            Repaint();


    }

    private void CreateGUIStyles()
    {
        // Custom gui style for wrapping text within the subtitle field
        // Again because its an editor window this is more manual creation 
        wrappingTextStyle = new GUIStyle(EditorStyles.textField);
        wrappingTextStyle.wordWrap = true;
    }

    private void HandleSubtitleCreation(string subtitleText, CharacterTag characterTag)
    {
        // Create new subtitle and add it to the manager list
        Subtitle newSubtitle = new Subtitle(subtitleText, characterTag);
        _subtitleManagerTarget._existingSubtitles.Add(newSubtitle);
    }

    // Since this is an editor window, we don't get the fancy editor "serializedObject" call
    // so we build it ourselves, automatically updates when the values are changed
    void HandleSerializedSubtitles()
    {
        // Set subtitle target as object field with type subtitlemanager
        // return if it doesnt exist
        _subtitleManagerTarget = (SubtitleManager)EditorGUILayout.ObjectField("Subtitle Manager:", _subtitleManagerTarget, typeof(SubtitleManager), true);
        _subtitleManagerTarget = FindFirstObjectByType<SubtitleManager>();
        if (_subtitleManagerTarget == null) return;
        
        if (_serializedSubtitleMTarget == null || _serializedSubtitleMTarget.targetObject != _subtitleManagerTarget)
        {
            _serializedSubtitleMTarget = new SerializedObject(_subtitleManagerTarget);
            _serializedSubtitleProperty = _serializedSubtitleMTarget.FindProperty("_existingSubtitles");
        }
        
        _serializedSubtitleMTarget.Update();
        EditorGUILayout.PropertyField(_serializedSubtitleProperty, true);
        _serializedSubtitleMTarget.ApplyModifiedProperties();
    }
}
