using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textOptions;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference selectAction;


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

        if (selectAction.action.WasPressedThisFrame())
        {
            GoToSelection();
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

    private void GoToSelection()
    {
        switch (_currentSelection)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                SceneManager.LoadScene(3);
                break;
            case 2:
                SceneManager.LoadScene(2);
                break;
            case 3:
                SceneManager.LoadScene(4);
                break;
            
        }
    }
}
