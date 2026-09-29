using UnityEngine;
using UnityEngine.InputSystem;

public class CurrentSettings : MonoBehaviour
{
    InputAction save;

    public string json;

    public SettingsState sett = new SettingsState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        save = InputSystem.actions.FindAction("Save JSON");
        sett.cameraSensitivity = 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
            //sett.cameraSensitivity = 0.2f;

        if (save.IsPressed())
        {
            SaveJSON(sett);
        }
        LoadJSON(sett);
    }

    public void SaveJSON(SettingsState setti)
    {
        //move this to its own section later
        sett.cameraSensitivity = 0.2f;
        json = JsonUtility.ToJson(setti); 
    }

    public void LoadJSON(SettingsState setti)
    {
        setti = JsonUtility.FromJson<SettingsState>(json);
        
    }
}
