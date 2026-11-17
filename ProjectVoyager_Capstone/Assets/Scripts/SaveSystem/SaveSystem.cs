using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum JsonSavePath
{
    Subtitles,
    Options
};

// Wrapper for list objects, generic so we can use it for anything
[Serializable]
public class SaveWrapper<T>
{
    public List<T> savedObjects;

    public SaveWrapper(List<T> objects)
    {
        this.savedObjects = objects;
    }
}

public static class SaveSystem 
{
    /// <summary>
    /// Saves a singular object to a given folder name and file name
    /// </summary>
    /// <param name="objectToSave"> Object to save</param>
    /// <param name="savePath"> Name of the folder to save to | should be the JSON subfolder name</param>
    /// <param name="fileName"> Name of the file to save to, without extension</param>
    /// <typeparam name="T"> Type of data that is being saved</typeparam>
    public static void Save<T>(T objectToSave, JsonSavePath savePath, string fileName)
    {
        string jsonString = JsonUtility.ToJson(objectToSave);
        File.WriteAllText(GetSavePathToFile(savePath, fileName), jsonString);
    }
    
    /// <summary>
    /// Saves JSON list data to a given folder name and file name
    /// </summary>
    /// <param name="objectsToSave"> List of objects to save</param>
    /// <param name="savePathType"> Name of the folder to save to | should be the JSON subfolder name</param>
    /// <param name="fileName"> Name of the file to save to, without extension</param>
    /// <typeparam name="T"> Type of data that is being saved</typeparam>
    public static void Save<T>(List<T> objectsToSave, JsonSavePath savePathType, string fileName)
    {
        // Creates wrapper for saved objects
        // We use this as if we were to individually loop through all obj in the given list,
        // JSON cannot read it back as the syntax is wrong
        SaveWrapper<T> wrapper = new SaveWrapper<T>(objectsToSave);
        
        // Save as JSON string
        string jsonString = JsonUtility.ToJson(wrapper, true);
        
        // Write string to given file path
        File.WriteAllText(GetSavePathToFile(savePathType, fileName), jsonString);
        
    }

    /// <summary>
    /// Loads JSON list data from a given folder name and file name
    /// </summary>
    /// <param name="savePathType"> JsonSavePath enum type | should be the JSON subfolder name</param>
    /// <param name="fileName"> Name of the file to load from, without extension</param>
    /// <typeparam name="T"> Type of data that is being loaded</typeparam>
    /// <returns>List of saved objects, regardless of type</returns>
    public static List<T> Load<T>(JsonSavePath savePathType, string fileName)
    {
        string path = GetSavePathToFile(savePathType, fileName);

        if (!File.Exists(path))
        {
            Debug.LogAssertion($"Save System did not find a file at: {path}");
            return new List<T>();
        }

        // Read json data at path
        string jsonString = File.ReadAllText(path);

        // Create wrapper with json data
        SaveWrapper<T> wrapper = JsonUtility.FromJson<SaveWrapper<T>>(jsonString);

        // Return wrappers saved objects as a list
        return wrapper.savedObjects;
    }

    // Returns the save path from a given type and a filename
    private static string GetSavePathToFile(JsonSavePath savePathType, string fileName)
    {
        // Start in the Json folder cause all json data should be located there
        string jsonPath = Path.Combine(Application.dataPath, "Json");
        string savePath = string.Empty;
        
        // Add cases here for new save types, this designates the folder name under Json in Assets folder
        // E.g. To add control options, 
        switch (savePathType)
        {
            case JsonSavePath.Subtitles:
                savePath = "Subtitles";
                break;
            case JsonSavePath.Options:
                savePath = "Options";
                break;
        }
        
        // Create new path with given type
        string newPath = Path.Combine(jsonPath, savePath);
        
        // Ensure folder exists
        Directory.CreateDirectory(newPath);
    
        // return filepath
        return Path.Combine(newPath, $"{fileName}.json");
    }
}
