using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomPopulateInBounds : MonoBehaviour
{
    [Header("Area Settings")]
    public BoxCollider2D areaCollider;

    [Header("Prefabs to Spawn")]
    public GameObject[] prefabs;

    [Header("Spawn Settings")]
    public int numberOfObjects = 10;
    // Start is called before the first frame update
    void Start()
    {
        PopulateArea();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateArea()
    {
        // Check if the collider is assigned
        if (areaCollider == null)
        {
            Debug.LogError("Area Collider (BoxCollider2D) is not assigned.");
            return;
        }

        // Check if the prefabs array is not empty
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned to spawn.");
            return;
        }

        // Calculate the bounds of the collider
        Vector2 center = (Vector2)areaCollider.transform.position + areaCollider.offset;
        Vector2 size = areaCollider.size;

        float minX = center.x - size.x / 2f;
        float maxX = center.x + size.x / 2f;
        float minY = center.y - size.y / 2f;
        float maxY = center.y + size.y / 2f;

        // Spawn the objects
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Select a random prefab
            GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

            // Generate a random position within the bounds
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // Instantiate the prefab at the random position
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
