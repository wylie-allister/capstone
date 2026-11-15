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
    private float _positionOffsetSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDisplayed)
        {
            popupTimer = 0.0f;
            return;
        }

        popupTimer += Time.deltaTime;

        if (popupObject.transform.position != _finalPosition)
        {
            popupObject.transform.position = Vector3.Lerp(popupObject.transform.position, _finalPosition,
                _positionOffsetSpeed * Time.deltaTime);
        }
        
        if (popupTimer >= maxDisplayTime)
            Hide();

    }

    public void Display()
    {
        isDisplayed = true;
        popupObject = Instantiate(popupPrefab, popupParent.transform);
        _finalPosition = popupObject.transform.position;
        popupObject.transform.position = new Vector3(_finalPosition.x - 20, _finalPosition.y, _finalPosition.z);
        popupObject.SetActive(true);
    }

    public void Hide()
    {
        if (popupObject != null)
            Destroy(popupObject);

        isDisplayed = false;
    }
    
}
