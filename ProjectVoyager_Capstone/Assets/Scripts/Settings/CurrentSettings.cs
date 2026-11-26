using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurrentSettings : MonoBehaviour
{
    InputAction save;
    InputAction toggleMenu;
    InputAction esc;

    public Slider camSens;
    public Slider mVol;
    public string json; 
    public GameObject obMenu;

    public GameObject genMenu;
    public GameObject auMenu;
    public GameObject viMenu;
    public GameObject HUDMenu;
    public GameObject rbMenu;
    

    public bool isOpen = false;
    public GameObject saveButton;

    //public GameObject lineButtons;

    private bool _toggle = false;
 

    public SettingsState sett = new SettingsState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        save = InputSystem.actions.FindAction("Save JSON");
        toggleMenu = InputSystem.actions.FindAction("Toggle Menu");
        esc = InputSystem.actions.FindAction("Escape");
        sett.cameraSensitivity = 0.2f;
        
        obMenu.SetActive(false);
       // saveButton.SetActive(false);
        //lineButtons.SetActive(false);

        sett.masterVolume = 1.0f;
        mVol.value = 1.0f;
        camSens.value = 0.2f;

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
        EscapeMenu();

    }

    public void ToggleIsOpen()
    {
        isOpen = false;
    }
    public void EscapeMenu()
    {

        if (esc.WasPressedThisFrame() && !rbMenu.activeSelf && isOpen)
        {
            genMenu.SetActive(false);
            auMenu.SetActive(false); 
            viMenu.SetActive(false);
            HUDMenu.SetActive(false);
            obMenu.SetActive(true);
        }



        if (esc.WasPressedThisFrame() && rbMenu.activeSelf)
        {
            genMenu.SetActive(true);
            rbMenu.SetActive(false);
        }
    }

    //this should be done with enums tbh
  /*  public void Controls()
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
    } */

    public void SaveButton()
    {
        SaveJSON(sett);
        isOpen = false;
        
        obMenu.SetActive(isOpen);
        /*
        auMenu.SetActive(isOpen);
        viMenu.SetActive(isOpen);
        HUDMenu.SetActive(isOpen);
        */
        //saveButton.SetActive(isOpen);
        //lineButtons.SetActive(isOpen);
    }

    public void SaveJSON(SettingsState setti)
    {
        //move this to its own section later
        sett.cameraSensitivity = camSens.value;
        sett.masterVolume = mVol.value;
        json = JsonUtility.ToJson(setti); 
    }

    public void LoadJSON(SettingsState setti)
    {
        setti = JsonUtility.FromJson<SettingsState>(json);
        
    }

    // Shitty form of manual toggle -bc used for start menu settings
    public void ManualToggle()
    {
        _toggle = true;
        if (!_toggle) return;
        
        isOpen = !isOpen;
        obMenu.SetActive(isOpen);
        //saveButton.SetActive(isOpen);
        //lineButtons.SetActive(isOpen);
        
        
        _toggle = false;
    }

    public void Menu()
    {
        if (toggleMenu.WasPerformedThisFrame())
        {
            isOpen = !isOpen;
            obMenu.SetActive(isOpen);
            //saveButton.SetActive(isOpen);
            //lineButtons.SetActive(isOpen);
        }
    }
}
