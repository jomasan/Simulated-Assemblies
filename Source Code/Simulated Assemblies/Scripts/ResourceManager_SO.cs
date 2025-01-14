using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Resource Manager", menuName = "Simulated Assemblies/Resource Manager")]
public class ResourceManager_SO : ScriptableObject
{
    public List<Resource> resources;

    // Method to get a resource by name
    public Resource GetResourceByName(string name)
    {
        return resources.Find(resource => resource.resourceName == name);
    }
}
