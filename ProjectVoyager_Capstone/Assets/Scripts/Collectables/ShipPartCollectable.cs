using UnityEngine;

public class ShipPartCollectable : MonoBehaviour
{
    public float rotSpeed = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, 1f * rotSpeed * Time.deltaTime);
    }
}
