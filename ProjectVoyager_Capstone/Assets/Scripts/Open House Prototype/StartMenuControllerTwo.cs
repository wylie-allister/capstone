using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartMenuControllerTwo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textOptions;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private InputActionReference backAction;
    [SerializeField] private CurrentSettings currentSettings;
    [SerializeField] private GameObject creditImage;

    private bool _toggleCredits = false;
    private int _currentSelection = 0;
    private bool canInput = true;
    private bool _canSelect = true;

    private float _defaultFontSize;
    [SerializeField] private float selectedFontSize;

    private void Start()
    {
        _defaultFontSize = textOptions[0].fontSize;
      
    }

    private void Update()
    {
        creditImage.SetActive(_toggleCredits);
        HandleSelection();

        if (moveAction.action.ReadValue<Vector2>().y > 0 && _currentSelection > 0 && canInput && _canSelect)
        {
            _currentSelection--;
            canInput = false;
        }
        else if (moveAction.action.ReadValue<Vector2>().y < 0 && _currentSelection < textOptions.Length - 1 && canInput && _canSelect)
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
        // Text -------------------------------------------------------
        // Changes text to reflect what option is currently selected
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
        
        // Selection revert
        // If we cant select, break from selection
        if (!_canSelect)
        {
            // If we press back, revert
            if (backAction.action.WasPressedThisFrame())
            {
                switch (_currentSelection)
                {
                    case 0:
                        break;
                    case 1:
                        currentSettings.ManualToggle();
                        _canSelect = true;
                        break;
                    case 2:
                        _toggleCredits = false;
                        _canSelect = true;
                        break;
                    case 3: 
                        break;
                    
                }
            }
        }
    }

    private void GoToSelection()
    {
        
        switch (_currentSelection)
        {
            case 0:
                //SceneManager.LoadScene(1);
                break;
            case 1:
                // Open settings, pause menu input
                currentSettings.ManualToggle();
                _canSelect = false;
                break;
            case 2:
                _toggleCredits = true;
                _canSelect = false;
                break;
            case 3:
                Application.Quit();
                //SceneManager.LoadScene(4);
                break;
            
        }
    }
}
