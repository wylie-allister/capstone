using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    public float playerSpeed;
    
    
    void Start()
    {
        //_playerCharacter = InstanceManager.Instance.playerCharacter;
        //InstanceManager.Instance.InitializeCheck();
    }

    void Update()
    { 
        UpdateText();
    }

    void UpdateText()
    {
        // Instance manager having STRUGGLES checking this?????? -bc fix later
        //playerSpeed = InstanceManager.Instance.playerCharacter.PlayerVelocity.magnitude;
        //playerSpeedText.text = $"Player Speed: {playerSpeed}";
    }
}
