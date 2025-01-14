using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class move : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public InputAction playerControls;
    Vector2 moveDirection = Vector2.zero;


    void onEnable(){
        playerControls.Enable();
    }

    void onDisable(){
        playerControls.Disable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(moveDirection);
    }

    private void FixedUpdate(){
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed );
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        //Debug.Log(value);
        moveDirection = value;
    }
}
