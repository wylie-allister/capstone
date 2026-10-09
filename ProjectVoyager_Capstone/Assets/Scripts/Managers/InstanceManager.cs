using UnityEngine;

/// <summary>
/// Ultimately this should serve as the ONLY singleton within the Unity project.
/// </summary>
public class InstanceManager : MonoBehaviour
{
    public HapticController hapticController;
    public LevelManager levelManager;
    public PlayerCharacter playerCharacter;
    
    // Initializers should be placed within this method
    private InstanceManager()
    {
        
    }

    private static InstanceManager _Instance;

    public static InstanceManager Instance
    {
        get
        {
            if (_Instance = null)
            {
                _Instance = new InstanceManager();
            }

            return _Instance;
        }
    }
}
