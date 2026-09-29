using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurrentSettings : MonoBehaviour
{
    InputAction save;
    InputAction toggleMenu;
    public Slider camSens;
    public string json;
    public GameObject menu;
    public bool isOpen = false;

    public SettingsState sett = new SettingsState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        save = InputSystem.actions.FindAction("Save JSON");
        toggleMenu = InputSystem.actions.FindAction("Toggle Menu");
        sett.cameraSensitivity = 0.1f;
        menu.SetActive(false);

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
        Menu();
    }

    public void SaveJSON(SettingsState setti)
    {
        //move this to its own section later
        sett.cameraSensitivity = camSens.value;
        json = JsonUtility.ToJson(setti); 
    }

    public void LoadJSON(SettingsState setti)
    {
        setti = JsonUtility.FromJson<SettingsState>(json);
        
    }

    public void Menu()
    {
        if (toggleMenu.WasPerformedThisFrame())
        {
            isOpen = !isOpen;
            menu.SetActive(isOpen);
        }
    }
}
