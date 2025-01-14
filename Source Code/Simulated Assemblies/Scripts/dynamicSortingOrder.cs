using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class dynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public bool invertOrder = false;

    // Optional offset to fine-tune sorting order
    public int sortingOrderOffset = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Calculate sorting order based on y position
        //spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + sortingOrderOffset;
        
        // Calculate sorting order based on the lowest point of the sprite
        float lowestYPosition = spriteRenderer.bounds.min.y;

        // Determine the base sorting order from the y position
        int sortingOrder = -(int)(lowestYPosition * 100) + sortingOrderOffset;

        // Invert the sorting order if needed
        if (invertOrder)
        {
            sortingOrder = -sortingOrder;
        }

        // Apply the calculated sorting order to the sprite renderer
        spriteRenderer.sortingOrder = sortingOrder;

        //spriteRenderer.sortingOrder = -(int)(lowestYPosition * 100) + sortingOrderOffset;
    }
}