using UnityEngine;

public class ResourceSink : ResourceNode
{
    public Resource resourceToConsume;        // The resource this sink consumes
    public int consumptionAmount = 1;         // Amount consumed per cycle
    public float consumptionInterval = 5f;    // Time between consumption cycles in seconds

    private float consumptionTimer = 0f;

    void Update()
    {
        consumptionTimer += Time.deltaTime;

        if (consumptionTimer >= consumptionInterval)
        {
            ConsumeResourceFromStorage();
            consumptionTimer = 0f;
        }
    }

    void ConsumeResourceFromStorage()
    {
        bool success = ConsumeResource(resourceToConsume, consumptionAmount);

        if (success)
        {
            Debug.Log($"{gameObject.name}: Successfully consumed {consumptionAmount} of {resourceToConsume.resourceName}");
        } else
        {
            Debug.LogWarning($"{gameObject.name}: Failed to consume {consumptionAmount} of {resourceToConsume.resourceName}");
        }
    }
}
