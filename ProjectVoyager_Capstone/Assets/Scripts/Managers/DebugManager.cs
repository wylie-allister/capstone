using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private InstanceManager _InstanceManager;
    [SerializeField] private TextMeshProUGUI playerSpeedText;
    public float playerSpeed;
    
    
    void Start()
    {
        _InstanceManager = InstanceManager.Instance;
    }

    void Update()
    {
        
    }
}
