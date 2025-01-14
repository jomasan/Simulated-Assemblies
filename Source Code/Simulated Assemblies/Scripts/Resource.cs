using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Simulated Assemblies/Resource")]
public class Resource : ScriptableObject
{
    public string resourceName;
    public Sprite icon;
    public GameObject resourcePrefab; // Reference to the prefab representing the resource
}

