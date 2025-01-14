using UnityEngine;

public class ResourceProducer : ResourceNode
{
    public Resource resourceToProduce;        // The resource this producer generates
    public int productionAmount = 1;          // Amount produced per cycle
    public float productionInterval = 5f;     // Time between production cycles in seconds

    private float productionTimer = 0f;

    public bool spawnResourcePrefab = true;

    // Position offset for spawning the resource prefabs
    private Vector3 spawnOffset = Vector3.zero;
    public float spawnRadius = 1f;

    void Update()
    {
        productionTimer += Time.deltaTime;

        if (productionTimer >= productionInterval)
        {
            ProduceResource();
            productionTimer = 0f;
        }
    }

    void ProduceResource()
    {
        // Add the produced resources to the local storage
        AddResource(resourceToProduce, productionAmount);

        // Optional: Visual or audio feedback
        //Debug.Log($"{gameObject.name}: Produced {productionAmount} of {resourceToProduce.resourceName}");

        if (spawnResourcePrefab )
        {
            // Instantiate the resource prefab
            InstantiateResourcePrefabs();
        }
    }

    void InstantiateResourcePrefabs()
    {
        if (resourceToProduce.resourcePrefab != null)
        {
            for (int i = 0; i < productionAmount; i++)
            {

                // Generate a random point inside a circle
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

                // Use the random circle coordinates for X and Z axes
                Vector3 randomOffset = new Vector3(randomCircle.x, randomCircle.y, 0);

                Vector3 spawnPosition = transform.position + randomOffset;
                GameObject resourceInstance = Instantiate(resourceToProduce.resourcePrefab, spawnPosition, Quaternion.identity);

                // Optional: Set parent to keep the hierarchy organized
                //resourceInstance.transform.parent = this.transform;

                // Optional: If the resource prefab has a script, initialize it here
                // For example, set the resource type or amount
                // ResourceObject resourceObject = resourceInstance.GetComponent<ResourceObject>();
                // if (resourceObject != null)
                // {
                //     resourceObject.Initialize(resourceToProduce, 1);
                // }
            }
        } else
        {
            //Debug.LogWarning($"{gameObject.name}: Resource prefab is not assigned for {resourceToProduce.resourceName}");
        }
    }
}
