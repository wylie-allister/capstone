using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManipulator : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;

    private Vector3 _originalSpawnPosition;
    private Vector3 _currentSpawnPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalSpawnPosition = spawnLocation.position;
    }

    // Update is called once per frame
    void Update()
    {
    #if UNITY_EDITOR
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ResetSpawnLocation();
        }
    #endif
    }

    public void UpdateSpawnLocation(Vector3 newSpawnLocation)
    {
        spawnLocation.position = newSpawnLocation;
    }

    public void ResetSpawnLocation()
    {
        spawnLocation.position = _originalSpawnPosition;
    }
}
