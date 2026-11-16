using UnityEngine;
using UnityEngine.UI;

public class InstructionPopup : MonoBehaviour
{
    public bool isDisplayed = false;

    [SerializeField] 
    private GameObject popupPrefab;
    [SerializeField] 
    private GameObject popupParent;

    [SerializeField]
    private float maxDisplayTime = 3.0f;


    private GameObject popupObject;

    private float popupTimer = 0.0f;
    private Vector3 _finalPosition;
    private float _positionOffsetSpeed = 10.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If not displayed reset timer and break
        if (!isDisplayed)
        {
            popupTimer = 0.0f;
            return;
        }

        // if displayed increment timer
        popupTimer += Time.deltaTime;

        // lerp popup position to final pos
        if (popupObject.transform.position != _finalPosition)
        {
            popupObject.transform.position = Vector3.Lerp(popupObject.transform.position, _finalPosition,
                _positionOffsetSpeed * Time.deltaTime);
        }
        
        // hide if over max display time
        if (popupTimer >= maxDisplayTime)
            Hide();

    }

    public void Display()
    {
        // Set display, instantiate popup, get final pos at instantiation point, move popup to side
        isDisplayed = true;
        popupObject = Instantiate(popupPrefab, popupParent.transform);
        _finalPosition = popupObject.transform.position;
        popupObject.transform.position = new Vector3(_finalPosition.x - 300, _finalPosition.y, _finalPosition.z);
        popupObject.SetActive(true);
    }

    public void Hide()
    {
        // if theres a popup, destroy it and disable display
        if (popupObject != null)
            Destroy(popupObject);

        isDisplayed = false;
    }
    
}
