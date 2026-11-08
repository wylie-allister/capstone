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
    public GameObject obMenu;
    public GameObject auMenu;
    public GameObject viMenu;
    public GameObject HUDMenu;
    

    public bool isOpen = false;
    public GameObject saveButton;

    public GameObject lineButtons;
 

    public SettingsState sett = new SettingsState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        save = InputSystem.actions.FindAction("Save JSON");
        toggleMenu = InputSystem.actions.FindAction("Toggle Menu");
        sett.cameraSensitivity = 0.1f;
        
        obMenu.SetActive(false);
        saveButton.SetActive(false);
        lineButtons.SetActive(false);

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

    //this should be done with enums tbh
    public void Controls()
    {
        obMenu.SetActive(true);
        auMenu.SetActive(false);
        viMenu.SetActive(false);
        HUDMenu.SetActive(false);
    }

    public void Audio()
    {
        obMenu.SetActive(false);
        auMenu.SetActive(true);
        viMenu.SetActive(false);
        HUDMenu.SetActive(false);
    }

    public void Visual()
    {
        obMenu.SetActive(false);
        auMenu.SetActive(false);
        viMenu.SetActive(true);
        HUDMenu.SetActive(false);
    }

    public void HUD()
    {
        obMenu.SetActive(false);
        auMenu.SetActive(false);
        viMenu.SetActive(false);
        HUDMenu.SetActive(true);
    }

    public void SaveButton()
    {
        SaveJSON(sett);
        isOpen = false;
        obMenu.SetActive(isOpen);
        auMenu.SetActive(isOpen);
        viMenu.SetActive(isOpen);
        HUDMenu.SetActive(isOpen);
        saveButton.SetActive(isOpen);
        lineButtons.SetActive(isOpen);
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
            obMenu.SetActive(isOpen);
            saveButton.SetActive(isOpen);
            lineButtons.SetActive(isOpen);
        }
    }
}
