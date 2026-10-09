using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textOptions;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private InputActionReference moveAction;


    private int _currentSelection = 0;
    private bool canInput = true;

    private float _defaultFontSize;
    [SerializeField] private float selectedFontSize;

    private void Start()
    {
        _defaultFontSize = textOptions[0].fontSize;
      
    }

    private void Update()
    {

        HandleSelection();

        if (moveAction.action.ReadValue<Vector2>().y > 0 && _currentSelection > 0 && canInput)
        {
            _currentSelection--;
            canInput = false;
        }
        else if (moveAction.action.ReadValue<Vector2>().y < 0 && _currentSelection < textOptions.Length - 1 && canInput)
        {
            _currentSelection++;
            canInput = false;
        }
        else if (moveAction.action.ReadValue<Vector2>().y == 0)
        {
            canInput = true;
        }
    }

    private void HandleSelection()
    {
        for (int i = 0; i < textOptions.Length; i++)
        {
            // Selected Text
            if (i == _currentSelection)
            {
                textOptions[i].color = selectedColor;
                textOptions[i].fontSize = selectedFontSize;
            }
            // Deselected Text
            else
            {
                textOptions[i].color = deselectedColor;
                textOptions[i].fontSize = _defaultFontSize;
            }    
            
        }
    }

    private void GoToStart()
    {

    }
}
