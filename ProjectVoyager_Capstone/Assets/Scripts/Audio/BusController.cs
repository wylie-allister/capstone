using UnityEngine;

public class BusController : MonoBehaviour
{

    public CurrentSettings cs;
    public FMOD.Studio.Bus mBus;
    string mBS = "Bus:/";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cs = GameObject.Find("SettingsHandler").GetComponent<CurrentSettings>();
        mBus = FMODUnity.RuntimeManager.GetBus(mBS);
    }

    // Update is called once per frame
    void Update()
    {
        mBus.setVolume(cs.sett.masterVolume);
    }
}
