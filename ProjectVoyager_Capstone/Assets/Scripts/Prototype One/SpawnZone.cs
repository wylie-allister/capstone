using UnityEngine;

public class SpawnZone : MonoBehaviour
{

    public Vector3 spawnPosition;

    [SerializeField] private Transform spawnTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If a custom spawn position exists, use that position data instead of the zone transform
        spawnPosition = spawnTransform != null ? spawnTransform.position : transform.position;
    }

}
