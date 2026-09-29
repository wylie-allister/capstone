using UnityEngine;
using UnityEngine.InputSystem;

public class CurrentSettings : MonoBehaviour
{
    SettingsState sett = new SettingsState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        sett.playerSpeed = 20f;

       SaveJSON();
    }

    public void SaveJSON()
    {

       
            string json = JsonUtility.ToJson(sett);
        
    }
}
