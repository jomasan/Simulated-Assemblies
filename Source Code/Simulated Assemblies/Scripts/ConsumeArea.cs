using System.Collections.Generic;
using UnityEngine;

public class ConsumeArea : MonoBehaviour
{
    // Messages to display when the player or resource enters or exits
    //public string resourceEnterMessage = "Resource entered the area.";

    public int regionID = 0;
    public creationManager cManager;

    // Optional: Reference to a specific required resource
    //public Resource requiredResource;
    //public int requiredAmount = 1; // For future use if needed

    //public List<GameObject> areaBuffer = new List<GameObject>();
    public List <GameObject> areaContains = new List<GameObject>();

    public List <Resource> requirements = new List<Resource>();

    public int gridColumns = 4;   // Number of columns in the grid
    //public float spacing = 1.5f;  // Spacing between objects in the grid
    private BoxCollider2D boxCollider;
    private float spacingX;
    private float spacingY;

    private void Start()
    {
        cManager = GameObject.FindObjectOfType<creationManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateSpacing();
    }

    private void Update()
    {
        RemoveMatchingResources();
        ArrangeObjectsInGrid();
    }

    // Called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TagUtilities.HasTag(other.gameObject, TagType.Resource))
        {
            {
                Debug.Log("Resource Entered of Type: " + TagType.Resource.ToString());
                areaContains.Add(other.gameObject);
            }
        }
    }

    // Called when another collider exits the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (TagUtilities.HasTag(other.gameObject, TagType.Resource))
        {
            {
                areaContains.Remove(other.gameObject);
            }
        }
    }

    private void CalculateSpacing()
    {
        // Get the collider bounds for width and height, factoring in the GameObject's scale
        float boxWidth = boxCollider.size.x * transform.localScale.x;
        float boxHeight = boxCollider.size.y * transform.localScale.y;

        // Calculate the number of rows based on the number of items and columns
        int gridRows = Mathf.CeilToInt(areaContains.Count / (float)gridColumns);

        // Ensure that spacing is calculated based on available space and the number of items
        spacingX = boxWidth / Mathf.Max(gridColumns, 1);
        spacingY = boxHeight / Mathf.Max(gridRows, 1);
    }

    public void ArrangeObjectsInGrid()
    {
        if (areaContains.Count == 0)
        {
            Debug.LogWarning("No objects in areaContains to arrange in grid.");
            return;
        }

        // Calculate the number of rows based on the number of objects and grid columns
        int gridRows = Mathf.CeilToInt(areaContains.Count / (float)gridColumns);

        // Calculate the starting position to center the grid within the BoxCollider2D
        Vector3 startPosition = transform.position - new Vector3(
            (gridColumns - 1) * spacingX / 2,
            (gridRows - 1) * spacingY / 2,
            0);

        // Arrange each object in the grid
        for (int i = 0; i < areaContains.Count; i++)
        {
            // Calculate the row and column for the current object
            int row = i / gridColumns;
            int col = i % gridColumns;

            // Calculate the new position based on row, column, and spacing
            Vector3 newPosition = startPosition + new Vector3(col * spacingX, row * spacingY, 0);

            // Update the object's position
            areaContains[i].transform.position = newPosition;
        }

        Debug.Log("Objects have been arranged in a grid within the collider.");
    }

    public void RemoveMatchingResources()
    {
        if (AreAllRequirementsMet())
        {
            //Debug.Log("All Requirements met!");
            // Loop through areaContains in reverse to safely remove items while iterating
            for (int i = areaContains.Count - 1; i >= 0; i--)
            {
                GameObject obj = areaContains[i];
                ResourceObject resourceObject = obj.GetComponent<ResourceObject>();

                // Check if the GameObject has ResourceInfo and if its resource matches any in requirements
                if (resourceObject != null && requirements.Contains(resourceObject.resourceType))
                {
                    areaContains.RemoveAt(i);
                    Destroy(obj);
                    Debug.Log($"Removed {obj.name} from areaContains because it matched a requirement.");
                    //Debug.Log("All Requirements met!");
                }
            }
        }
    }

    private bool AreAllRequirementsMet()
    {
        // Dictionary to count required resources
        Dictionary<Resource, int> requiredCounts = new Dictionary<Resource, int>();

        // Populate the dictionary with required resources and their counts
        foreach (Resource req in requirements)
        {
            if (requiredCounts.ContainsKey(req))
            {
                requiredCounts[req]++;
            } else
            {
                requiredCounts[req] = 1;
            }
        }

        // Dictionary to count resources in areaContains
        Dictionary<Resource, int> availableCounts = new Dictionary<Resource, int>();

        // Populate the dictionary with available resources and their counts
        foreach (GameObject obj in areaContains)
        {
            ResourceObject resourceInfo = obj.GetComponent<ResourceObject>();
            if (resourceInfo != null)
            {
                Resource resourceType = resourceInfo.resourceType;

                if (availableCounts.ContainsKey(resourceType))
                {
                    availableCounts[resourceType]++;
                } else
                {
                    availableCounts[resourceType] = 1;
                }
            }
        }

        // Check if all required resources and counts are met in availableCounts
        foreach (KeyValuePair<Resource, int> kvp in requiredCounts)
        {
            Resource requiredResource = kvp.Key;
            int requiredCount = kvp.Value;

            // Check if the resource is in availableCounts with sufficient quantity
            if (!availableCounts.ContainsKey(requiredResource) || availableCounts[requiredResource] < requiredCount)
            {
                return false; // Requirement not met
            }
        }

        return true; // All requirements met
    }


    // Called when another collider enters the trigger
    private void OnTriggerEnter2Dd(Collider2D other)
    {   
        /*
        //DESTROY SPECIFIC RESOURCE THAT IS CARRIED BY PLAYER:
        if (TagUtilities.HasTag(other.gameObject, TagType.Player))
        {
            playerController pc = other.GetComponent<playerController>();
            GameObject objBeingCarried = pc.GetObjectToGrab();
            if (objBeingCarried != null)
            {
                // Optional: Check if it's the required resource
                ResourceObject resourceObject = objBeingCarried.GetComponent<ResourceObject>();
                if (resourceObject != null)
                {
                    if (requiredResource == null || resourceObject.resourceType == requiredResource)
                    {
                        other.GetComponent<playerController>().DropAndDestroy();
                    }
                }
            }
        }
        */

        
        if (TagUtilities.HasTag(other.gameObject, TagType.Resource))
        {
            {
                Debug.Log("Resource Entered of Type: " + TagType.Resource.ToString());
                areaContains.Add(other.gameObject);
                // Get the ResourceObject component
                ResourceObject resourceObject = other.GetComponent<ResourceObject>();
                if (resourceObject != null)
                {
                    // Optional: Check if it's the required resource
                    //if (requiredResource == null || resourceObject.resourceType == requiredResource)
                    //{
                        // Consume the resource
                        //ConsumeResource(resourceObject);
                    //}
                }
            }
        }
        
    }

   



    // Method to consume the resource object
    private void ConsumeResource(ResourceObject resourceObject)
    {
        // Perform any actions needed upon consuming the resource
        Debug.Log($"Consumed resource: {resourceObject.resourceType.resourceName}");

        // Destroy the resource object
        Destroy(resourceObject.gameObject);

        // Optional: Trigger events, update UI, etc.
        // For example, inform the creationManager
        //cManager.ResourceConsumedInRegion(resourceObject.resourceType, regionID);

        // Optionally, you can keep track of how many resources have been consumed
        // and trigger further actions when certain conditions are met
    }
}
