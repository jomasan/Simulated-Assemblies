using UnityEngine;

public class drag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Rigidbody2D rigB;                  // Reference to the Collider2D component
    private bool previousIsKinematic;     // To store the previous isKinematic state

    void Start()
    {

        // Get the Collider2D component attached to this GameObject
        rigB = transform.GetComponent<Rigidbody2D>();
        if (rigB == null)
        {
            Debug.LogError("No Rigidbody2D component found on this GameObject.");
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Collision!");

        if (rigB == null)
            return;

        isDragging = true;

        // Store the previous isKinematic state
        previousIsKinematic = rigB.isKinematic;

        // Set the collider to be kinematic while dragging
        rigB.isKinematic = true;

        // Convert mouse position to world position and calculate offset
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mousePosition;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the object's position to follow the mouse, maintaining the offset
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition + offset;
        }
    }

    void OnMouseUp()
    {
        if (rigB == null)
            return;

        isDragging = false;

        // Restore the collider's isKinematic state
        rigB.isKinematic = previousIsKinematic;
    }
}