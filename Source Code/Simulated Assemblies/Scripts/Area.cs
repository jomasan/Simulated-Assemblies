using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Unity UI
using TMPro; // For TextMeshPro (optional, comment out if not using)
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class Area : MonoBehaviour
{
    // Messages to display when the player or resource enters or exits
    //public string resourceEnterMessage = "Resource entered the area.";

    //public int regionID = 0;
    private creationManager cManager;

    // Optional: Reference to a specific required resource
    //public Resource requiredResource;
    //public int requiredAmount = 1; // For future use if needed

    //public List<GameObject> areaBuffer = new List<GameObject>();
    public List<GameObject> areaContains = new List<GameObject>();

    public List<Resource> requirements = new List<Resource>();

    public enum areaType
    {
        Pool,
        Input,
        Output,
        Absorb,
        InputAbsorb
    }

    public float pullSpeed = 2f;
    public float pullDuration = 1f;
    public float pullRadius = 10f;
    private List<Transform> resourcesToAbsorb = new List<Transform>();
    public string resourceTag = "Resource";
    public int AbsorbedResourceCount = 0;
    public TMP_Text tmpText; // TextMeshPro UI


    public areaType TypeOfArea;

    public int gridColumns = 4;   // Number of columns in the grid
    //public float spacing = 1.5f;  // Spacing between objects in the grid
    private BoxCollider2D boxCollider;
    private float spacingX;
    private float spacingY;

    public bool consumeResources = true;
    public bool sortAsGrid = false;

    private void Start()
    {
        cManager = GameObject.FindObjectOfType<creationManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateSpacing();
    }

    private void Update()
    {
        if(consumeResources) RemoveMatchingResources();
        if(sortAsGrid) ArrangeObjectsInGrid();

        if (TypeOfArea == areaType.Absorb )
        {
            FindResourcesWithinRadius();
            //PullResources();
            PullResourcesWithTween();
            UpdateUI();
        }else if (TypeOfArea == areaType.InputAbsorb)
        {
            FindResourcesWithinRadius();
            PullResourcesWithTween();
        }
    }

    //very poor performance
    private void FindResourcesWithinRadius()
    {
        // Clear the list to avoid duplicates
        resourcesToAbsorb.Clear();

        // Find all resources in the scene with the specified tag
        //TagUtilities.HasTag(other.gameObject, TagType.Grabbable)
        //GameObject[] resources = GameObject.FindGameObjectsWithTag(resourceTag);
        Object[] resources = FindObjectsOfType(typeof(ResourceObject));

        foreach (Object r in resources)
        {
            GameObject resource = ((ResourceObject)r).gameObject;

            ResourceObject rObj = resource.GetComponent<ResourceObject>();
            
            if(rObj.resourceType == requirements[0])
            {   
                float distance = Vector2.Distance(transform.position, resource.transform.position);
                //resource.GetComponent<BoxCollider2D>().isTrigger = true;
                // Add to the list if within the pull radius
                if (distance <= pullRadius)
                {

                    if (resource.GetComponent<BoxCollider2D>() != null)
                    {
                        resource.GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                    if (resource.GetComponent<CircleCollider2D>() != null)
                    {
                        resource.GetComponent<CircleCollider2D>().isTrigger = true;
                    }

                    resourcesToAbsorb.Add(resource.transform);
                }
            }

            if (TagUtilities.HasTag(resource, TagType.Resource))
            {

            }

            
        }
    }
    private void PullResources()
    {
        foreach (Transform resource in resourcesToAbsorb)
        {
            if (resource == null) continue;

            // Calculate the direction to pull the resource
            Vector2 direction = (transform.position - resource.position).normalized;

            // Gradually move the resource towards the absorber
            resource.position += (Vector3)direction * pullSpeed * Time.deltaTime;
        }
    }
    private void PullResourcesWithTween()
    {
        foreach (Transform resource in resourcesToAbsorb)
        {
            if (resource == null) continue;

            // Start a coroutine for each resource to move it using the tweening function
            StartCoroutine(MoveResourceToAbsorber(resource));
        }

        // Clear the list after initiating movement
        resourcesToAbsorb.Clear();
    }
    IEnumerator MoveResourceToAbsorber(Transform resource)
    {
        Vector3 startPosition = resource.position;
        Vector3 targetPosition = transform.position;

        float duration = pullDuration; // Duration of the tween (adjust as needed)
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the normalized time (t)
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Apply an easing function from the Tween class
            float easedT = Tween.EaseInOutQuad(t); // Choose any easing function here
            //float easedT = Tween.EaseInQuad(t); // Choose any easing function here
            //float easedT = Tween.BounceBackEaseIn(t); // Choose any easing function here
            //float easedT = Tween.EaseOutBack(t); // Choose any easing function here
            
            // Interpolate the position based on eased time
            if (resource != null) resource.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null; // Wait for the next frame
        }

        // Ensure the resource snaps to the exact target position at the end
        if (resource != null) resource.position = targetPosition;

        // Optionally, destroy the resource after it reaches the absorber
        //Destroy(resource.gameObject);
    }


    private void UpdateUI()
    {
        tmpText.gameObject.SetActive(true);
        //tmpText.gameObject.transform.position = transform.position;

        if (tmpText != null)
        {
            tmpText.text = "0" + AbsorbedResourceCount.ToString();
        }
    }

    // Called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TagUtilities.HasTag(other.gameObject, TagType.Resource))
        {
            {
                //Debug.Log("Resource Entered of Type: " + TagType.Resource.ToString());
                areaContains.Add(other.gameObject);
            }
        }

        //ABSORB RESOURCES BASED ON TAG
        if (TypeOfArea == areaType.Absorb)
        {
            if (TagUtilities.HasTag(other.gameObject, TagType.Resource))
            {
                Destroy(other.gameObject);
                AbsorbedResourceCount++;
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
    public bool RemoveMatchingResources()
    {
        if (AreAllRequirementsMet())
        {
            // Create a copy of the requirements list to track which resources are still needed
            List<Resource> remainingRequirements = new List<Resource>(requirements);

            // Loop through areaContains in reverse to safely remove items while iterating
            for (int i = areaContains.Count - 1; i >= 0 && remainingRequirements.Count > 0; i--)
            {
                GameObject obj = areaContains[i];
                ResourceObject resourceObject = obj.GetComponent<ResourceObject>();

                // Check if the GameObject has ResourceObject and if its resource matches any in remainingRequirements
                if (resourceObject != null && remainingRequirements.Contains(resourceObject.resourceType))
                {
                    // Remove the resource type from the remainingRequirements
                    remainingRequirements.Remove(resourceObject.resourceType);

                    // Remove the GameObject from areaContains and destroy it
                    StopAllCoroutines();
                    areaContains.RemoveAt(i);
                    Destroy(obj);
                    Debug.Log($"Removed {obj.name} from areaContains because it matched a requirement.");
                }
            }

            return true; // All required resources have been removed
        } else
        {
            return false; // Not all requirements are met
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public Vector2 GetPositionWithRandomness(float scaleFactor)
    {
        
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(-1, 1);
        float randomZ = Random.Range(0, 0);
        Vector3 rand = new Vector3(randomX, randomY, randomZ);
        rand.Normalize();
        rand *= scaleFactor;
        Vector3 currentPos = transform.position;

        return currentPos += rand;
    }
    //NOT WORKING YET
    public Vector2 GetRandomPointInBounds()
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D is not assigned!");
            return Vector2.zero;
        }

        // Get the local size and offset of the BoxCollider2D
        Vector2 localSize = boxCollider.size;
        Vector2 localOffset = boxCollider.offset;

        // Adjust the size to account for the local scale of the GameObject
        Vector2 scaledSize = new Vector2(localSize.x * transform.localScale.x, localSize.y * transform.localScale.y);

        // Calculate the min and max points in local space, including the offset
        Vector2 localMin = localOffset - (scaledSize / 2);
        Vector2 localMax = localOffset + (scaledSize / 2);

        // Generate random x and y values within the local bounds
        float randomX = Random.Range(localMin.x, localMax.x);
        float randomY = Random.Range(localMin.y, localMax.y);

        // Convert the local random point to global coordinates
        Vector2 localRandomPoint = new Vector2(randomX, randomY);
        Vector2 transform2d = new Vector2(transform.position.x, transform.position.y);
        Vector2 randomAroundTransform = transform2d + localRandomPoint;
        //Vector2 globalRandomPoint = transform.TransformPoint(localRandomPoint);

        // Return the global random point
        return randomAroundTransform;
    }

    public bool AreAllRequirementsMet()
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

    // Method to consume the resource object
    private void ConsumeResource(ResourceObject resourceObject)
    {
        // Perform any actions needed upon consuming the resource
        Debug.Log($"Consumed resource: {resourceObject.resourceType.resourceName}");

        StopAllCoroutines();
        // Destroy the resource object
        Destroy(resourceObject.gameObject);

        // Optional: Trigger events, update UI, etc.
        // For example, inform the creationManager
        //cManager.ResourceConsumedInRegion(resourceObject.resourceType, regionID);

        // Optionally, you can keep track of how many resources have been consumed
        // and trigger further actions when certain conditions are met
    }
}
