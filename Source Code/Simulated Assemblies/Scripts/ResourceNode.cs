using UnityEngine;
using System.Collections.Generic;

public abstract class ResourceNode : MonoBehaviour
{
    // Dictionary to hold resource quantities
    protected Dictionary<Resource, int> resourceQuantities = new Dictionary<Resource, int>();

    // Method to add resources to the node
    public virtual void AddResource(Resource resource, int amount)
    {
        if (resourceQuantities.ContainsKey(resource))
        {
            resourceQuantities[resource] += amount;
        } else
        {
            resourceQuantities.Add(resource, amount);
        }

        //Debug.Log($"{gameObject.name}: Added {amount} of {resource.resourceName}. Total: {resourceQuantities[resource]}");
    }

    // Method to get the current quantity of a resource
    public virtual int GetResourceQuantity(Resource resource)
    {
        if (resourceQuantities.TryGetValue(resource, out int quantity))
        {
            return quantity;
        }
        return 0;
    }

    // Method to consume resources
    public virtual bool ConsumeResource(Resource resource, int amount)
    {
        int currentQuantity = GetResourceQuantity(resource);

        if (currentQuantity >= amount)
        {
            resourceQuantities[resource] -= amount;
            //Debug.Log($"{gameObject.name}: Consumed {amount} of {resource.resourceName}. Remaining: {resourceQuantities[resource]}");
            return true;
        } else
        {
            //Debug.LogWarning($"{gameObject.name}: Not enough {resource.resourceName} to consume. Required: {amount}, Available: {currentQuantity}");
            return false;
        }
    }

    // Method to transfer resources to another node
    public virtual bool TransferResource(Resource resource, int amount, ResourceNode targetNode)
    {
        if (ConsumeResource(resource, amount))
        {
            targetNode.AddResource(resource, amount);
            //Debug.Log($"{gameObject.name}: Transferred {amount} of {resource.resourceName} to {targetNode.gameObject.name}");
            return true;
        } else
        {
            //Debug.LogWarning($"{gameObject.name}: Failed to transfer {resource.resourceName} to {targetNode.gameObject.name}");
            return false;
        }
    }
}
