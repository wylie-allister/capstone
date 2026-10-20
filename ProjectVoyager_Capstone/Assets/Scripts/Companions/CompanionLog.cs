using Unity.VisualScripting;
using UnityEngine;

public class CompanionLog : MonoBehaviour
{

    int luck = 10;
    int skill = 20;
    int will = 15;

    bool hasTestCompanion;
    bool isHoldingTestCompanion;
    public bool canPickupCompanion;

    public GameObject testRat;

    private PlayerInputActions _inputActions;

    private GameObject lastInteracted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasTestCompanion = false;
        isHoldingTestCompanion = false;
        canPickupCompanion = false;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        HandleCompanions();
    }

    public void HandleCompanions()
    {
        var input = _inputActions.Gameplay;

        if (canPickupCompanion == true && input.Interact.WasPressedThisFrame())
        {
            testRat.SetActive(true);
            hasTestCompanion = true;
            lastInteracted.SetActive(false);
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
