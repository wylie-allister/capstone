using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CompanionLog : MonoBehaviour
{
    public TestCompanion tComp;

    bool isHoldingTestCompanion;
    //bool hasCompanion;
    public bool canPickupCompanion;
    bool hasTestComp;

    private PlayerInputActions _inputActions;

    private GameObject lastInteracted;

    public TextMeshProUGUI tmp;

    public GameObject[] companionList;
    int comp = 0;
    int stopRoll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //hasCompanion = false;
        isHoldingTestCompanion = false;
        canPickupCompanion = false;
        hasTestComp = false;
        stopRoll = companionList.Length;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        tComp.luck = 10;
        tComp.skill = 20;
        tComp.will = 15;

    }

    // Update is called once per frame
    void Update()
    {
        HandleCompanions();
        CompSwap();
        if (comp != 0 && hasTestComp == true)
        {
            isHoldingTestCompanion = false;
        }
        else if (comp == 0 && hasTestComp == true)
        {
            isHoldingTestCompanion = true;
        }
        TestCompEffects();

    }

    public void HandleCompanions()
    {
        var input = _inputActions.Gameplay;

        if (canPickupCompanion == true && input.Interact.WasPressedThisFrame())
        {
            companionList[0].SetActive(true);
            lastInteracted.SetActive(false);
            isHoldingTestCompanion = true;
            hasTestComp = true;
        }
        tmp.text = ($"Luck: {tComp.luck} \n Skill: {tComp.skill} \n Will: {tComp.will}");
    }

    public void TestCompEffects()
    {
        if (isHoldingTestCompanion == true)
        {
            tComp.luck = 15;
            tComp.skill = 25;
            tComp.will = 20;
          
        }
        
        if (isHoldingTestCompanion == false)
        {
            tComp.luck = 10;
            tComp.skill = 20;
            tComp.will = 15;
           
        }
    }

    public void CompSwap()
    {
        var input = _inputActions.Gameplay;

        if (input.Swap.WasPressedThisFrame())
        {
            companionList[comp].SetActive(false);
            comp++;
            if (comp >= stopRoll)
            {
                comp = 0;
            }
            companionList[comp].SetActive(true);
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Companion"))
        {
            canPickupCompanion = true;
            lastInteracted = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Companion"))
        {
            canPickupCompanion = false;
        }
    }
}
