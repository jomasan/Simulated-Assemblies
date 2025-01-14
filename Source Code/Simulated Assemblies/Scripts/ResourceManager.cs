using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public List<ResourceObject> allResources = new List<ResourceObject>();

    public List<ResourceUIBinding> resourcesToTrack = new List<ResourceUIBinding>();
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updatePanels();
    }

    public void updatePanels()
    {
        for (int i = 0; i < resourcesToTrack.Count; i++)
        {
            Resource toTrack = resourcesToTrack[i].resourceType;
            resourceInfoManager resourceIM = resourcesToTrack[i].resourceUIPanel;
            int count = GetResourceCount2(toTrack);
            resourceIM.resourceName.text = toTrack.resourceName.ToString();
            resourceIM.resourceAmount.text = count.ToString();
            //Debug.Log("Resource: " + toTrack.resourceName + " Amount: " + count + ", from: " + allResources.Count);
        }
    }

    // Methods to add and remove resources
    public void AddResource(ResourceObject resource)
    {
        allResources.Add(resource);
    }

    public void RemoveResource(ResourceObject resource)
    {
        //Debug.Log("Trying to remove resource: " + resource.resourceType.resourceName);
        if (allResources.Contains(resource))
        {   
            
            allResources.Remove(resource);
        }
    }

    public int GetResourceCount(Resource resourceType)
    {   
        return allResources.Count(r => r.resourceType == resourceType);
    }

    public int GetResourceCount2(Resource resourceType)
    {
        int count = 0;
        foreach (var resource in allResources)
        {
            if (resource.resourceType == resourceType)
            {
                count++;
            }
        }
        return count;
    }


    public Dictionary<Resource, int> GetAllResourceCounts()
    {
        return allResources
            .GroupBy(r => r.resourceType)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
}


