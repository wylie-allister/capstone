using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TreeSpawn : MonoBehaviour
{
    //I LOVE STEALING!!! YIPPEE!!!
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject[] treePrefabs;

    [Header("Settings")]
    public Transform treeParent;
    public int numberOfTrees;

    public Vector2 treeSpawnRadius;
    public List<GameObject> trees = new List<GameObject>();
    public float treeSpacingOffset = 2.0f;
    void Start()
    {

    }

    void Awake()
    {
        HandleTreeRandomization();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void HandleTreeRandomization()
    {
        // Create new list of trees with an individual random position
        for (int treeIndex = 0; treeIndex < numberOfTrees; treeIndex++)
        {
            Quaternion randomRotation = Quaternion.identity;
            randomRotation.y = Random.rotation.y;
            GameObject newTree = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)]);

            RandomizeTreePosition(newTree);
            newTree.transform.localScale = new Vector3(1, 1, 1) * Random.Range(0.8f, 1.2f);

            
                
  
            newTree.transform.rotation = randomRotation;
            newTree.transform.parent = treeParent;
            trees.Add(newTree);
        }
    }

    public void RandomizeTreePosition(GameObject tree)
    {
        // Get new random position within the spawn radius
        Vector3 randomPosition = new Vector3(Random.Range(-treeSpawnRadius.x, treeSpawnRadius.x), 3, Random.Range(-treeSpawnRadius.y, treeSpawnRadius.y));

        RaycastHit hit;
        if (Physics.Raycast(randomPosition, Vector3.down, out hit, 5f, LayerMask.GetMask("Ground")))
        {
            randomPosition.y = hit.point.y + 80f;
        }

        // Set tree position to new random position
        tree.transform.position = randomPosition;
        

        tree.transform.SetParent(treeParent);
    }
}
