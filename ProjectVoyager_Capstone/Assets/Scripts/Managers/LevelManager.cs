using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform playerSpawn;

    [SerializeField] private GameObject endTrigger;

    [SerializeField] private Scene[] gameScenes;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        if (playerSpawn == null || endTrigger == null) //|| gameScenes == null)
        {
            Debug.LogAssertion("LevelManager: Missing Reference");
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnPlayer()
    {
        InstanceManager.Instance.playerCharacter.SetPosition(playerSpawn.position);
    }
    
    //public void 
}
