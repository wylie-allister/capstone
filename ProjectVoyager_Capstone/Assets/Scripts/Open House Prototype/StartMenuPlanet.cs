using UnityEngine;

public class StartMenuPlanet : MonoBehaviour
{
    [SerializeField] 
    private float rotSpeed = 1.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, rotSpeed);
    }
}
