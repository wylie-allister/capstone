using UnityEngine;

public class BGMParams : MonoBehaviour
{

    [SerializeField]
    private GameObject bgmPrefab;
    [SerializeField]
    private GameObject bgmParent;
    public PlayerNoises pn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TSMMWKMS()
    {
        if (bgmPrefab.name == "Ch1" && pn.checkNum < 2)
        {
            pn.checkNum = 2;
        }

        if (bgmPrefab.name == "Ch2" && pn.checkNum < 3)
        {
            pn.checkNum = 3;
        }
        if (bgmPrefab.name == "Ch3" && pn.checkNum < 4)
        {
            pn.checkNum = 4;
        }

        if (bgmPrefab.name == "Ch4" && pn.checkNum < 5)
        {
            pn.checkNum = 5;
        }
        if (bgmPrefab.name == "Ch5" && pn.checkNum < 6)
        {
            pn.checkNum = 6;
        }

        if (bgmPrefab.name == "Ch6" && pn.checkNum < 7)
        {
            pn.checkNum = 7;
        }
        if (bgmPrefab.name == "Ch7" && pn.checkNum < 8)
        {
            pn.checkNum = 8;
        }

    }
}
