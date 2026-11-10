using UnityEngine;

public class WarpToStart : MonoBehaviour
{
    public GameObject player;
    public GameObject spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WarpBack"))
        {
            Debug.Log("AAAA");
            player.transform.position = spawn.transform.position;
        }
    }
}
