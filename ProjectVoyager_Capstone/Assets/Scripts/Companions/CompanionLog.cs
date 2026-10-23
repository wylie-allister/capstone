using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CompanionLog : MonoBehaviour
{
    public TestCompanion tComp;

    bool isHoldingTestCompanion;
    bool hasCompanion;
    public bool canPickupCompanion;

    public GameObject testRat;

    private PlayerInputActions _inputActions;

    private GameObject lastInteracted;

    public TextMeshProUGUI tmp;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasCompanion = false;
        isHoldingTestCompanion = false;
        canPickupCompanion = false;

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
        TestCompEffects();
        Debug.Log(tComp.luck);
    }

    public void HandleCompanions()
    {
        var input = _inputActions.Gameplay;

        if (canPickupCompanion == true && input.Interact.WasPressedThisFrame())
        {
            testRat.SetActive(true);
            lastInteracted.SetActive(false);
            isHoldingTestCompanion = true;
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
        else
        {
            tComp.luck = 10;
            tComp.skill = 20;
            tComp.will = 15;
           
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
