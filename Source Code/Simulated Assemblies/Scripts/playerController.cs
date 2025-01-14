using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Random = UnityEngine.Random; // Add this line
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.XR;

public class playerController : MonoBehaviour
{
    public float playerSpeed = 2.0f;
    public Rigidbody2D rb;
    private Vector2 playerVelocity;
    private Vector2 movementInput = Vector2.zero;
    private bool fired = false;
    public SpriteRenderer spriteRenderer;

    public creationManager cManager;
    public playersInfo pInfo;
    public int playerID = 0;
    
    public bool isCarryingObject = false;
    public Transform carryPosition; 
    public GameObject objectToGrab;
    
    public List <GameObject> listobjectsToGrab = new List<GameObject>();
    public int maxObjectsToCarry = 2;
    public bool isAbsorbingResources = false;

    private Vector3 grabOffset;

    public GameObject objectToLabor;
    public GameObject objectToInspect;
    public bool performingLabor = false;

    public int capital = 0;
    public List<GameObject> properties = new List<GameObject>();

    //Grab Point
    public float distanceFromPlayer = 0.5f; // Desired distance from the player
    public Vector3 grabPoint = Vector3.zero;
    public Vector3 grabAreaOffset = Vector3.zero;
    public GameObject grabArea;
    public GameObject grabGraphic;
    private Vector3 normFwd = Vector2.zero;

    //EVENTS / on fire 
    public UnityEvent onPlayerButton_A;
    public UnityEvent onPlayerButton_B;
    public UnityEvent onPlayerButton_X;
    public UnityEvent onPlayerButton_Y;


    private bool coroutineRunning = false;
    public bool doUpdate = false;
    public bool debug = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the sprite renderer component
        cManager = GameObject.FindObjectOfType<creationManager>(); // Get the creation manager component

        //add yourself to the player list
        pInfo = GameObject.FindObjectOfType<playersInfo>(); // Get the players info component
        playerID = pInfo.allPlayers.Count; // Set the player ID to the current number of players

        if (pInfo.playerColors[playerID] != null)
        {
            spriteRenderer.color = pInfo.playerColors[playerID]; // Set the color of the sprite renderer to the color of the player
        } else
        {
            spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f); // Set the color of the sprite renderer to a random color
        }
        
        pInfo.allPlayers.Add(transform.gameObject);
        pInfo.allControllers.Add(this);

    }

    //OUTLINE OF PLAYER ACTIONS:
    //-MOVE -> JOYSTICK             WORKING 
    //-GRAB / DROP ->               WORKING
    //-WORK / PRODUCE RESOURCE ->   WORKING
    //-CREATE STRUCTURE (FREE) ->   WORKING
    //-CREATE BY PURCHASE ->        NOT WORKING
    //-DESTROY STRUCTURE ->         NOT WORKING
    //-CLAIM OWNERSHIP ->           NOT WORKING
    //-LOCK / UNLOCK STRUCTURE ->   NOT WORKING

    //IMPROVEMENTS:
    //-PRECISE ACTION POINT ->      WORKING
    //-MULTI OBJECT GRAB ->          NOT WORKING
    void Update()
    {
        if(performingLabor)  PerformLabor();
        if(isAbsorbingResources) AbsorbResources();

    }
    public void calculateGrabPoint(Vector2 movement)
    {
        // Calculate the position at the specified distance in the player's forward direction
        if(movement != Vector2.zero) normFwd = movement.normalized;
        grabPoint = transform.position + grabAreaOffset + (normFwd * distanceFromPlayer);
        grabArea.transform.position = grabPoint;
        grabGraphic.transform.position = grabPoint;

        if (objectToInspect != null)
        {
            if(objectToInspect.GetComponent<Station>() != null)
            {   
                if(objectToInspect.GetComponent<Station>().inspectionPoint != null)
                {
                    Vector3 inspPoint = objectToInspect.GetComponent<Station>().inspectionPoint.position;
                    grabGraphic.transform.position = inspPoint; //magnetic graphic to collision objects
                    grabGraphicAnimation();

                } else
                {
                    Vector3 inspPoint = objectToInspect.GetComponent<Station>().transform.position;
                    grabGraphic.transform.position = inspPoint; //magnetic graphic to collision objects
                    grabGraphicAnimation();
                }
            } 
            else if (objectToInspect.GetComponent<ResourceObject>() != null)
            {
                Vector3 inspPoint = objectToInspect.GetComponent<ResourceObject>().transform.position;
                grabGraphic.transform.position = inspPoint; //magnetic graphic to collision objects
                grabGraphicAnimation();
            }
        }
    }
    public void grabGraphicAnimation()
    {
        if (!coroutineRunning && !isCarryingObject && !performingLabor && doUpdate)
        {
            doUpdate = false;
            float startScale = grabGraphic.transform.localScale.x + 2;
            float endScale = grabGraphic.transform.localScale.x;
            float duration = 0.5f;
            StartCoroutine(ScaleOverTime(grabGraphic, new Vector3(startScale, startScale, startScale), new Vector3(endScale, endScale, endScale), duration));
        }
    }
    public void onMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

        calculateGrabPoint(context.ReadValue<Vector2>());
    }
    public void onFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            fired = context.action.triggered;
            if (debug) Debug.Log("Button A Pressed!");
            onPlayerButton_A.Invoke();
        }
    }
    public void onFire2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            fired = context.action.triggered;
            if (debug) Debug.Log("Button B Pressed!");
            onPlayerButton_B.Invoke();
        }
    }
    public void onFire3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (debug) Debug.Log("Button X Pressed Down");
            StartLabor();
        } else if (context.performed)
        {
            if (debug) Debug.Log("Button X Held");
        } else if (context.canceled)
        {
            if (debug) Debug.Log("Button X Released");
            cancelLabor();
        }
    } 
    public void onFire4(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (debug) Debug.Log("Button Y Pressed Down");
            StartAbsorb();
        } else if (context.performed)
        {
            if (debug) Debug.Log("Button Y Held");
        } else if (context.canceled)
        {
            if (debug) Debug.Log("Button Y Released");
            StopAbsorb();
            releaseAbsorbedObjects();
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementInput.x * playerSpeed, movementInput.y * playerSpeed);
    }
    public void CreateObjects()
    {   
        if(cManager != null){
            if (debug) Debug.Log("Create Attempt by: " + transform.gameObject);
            cManager.CreateSpriteAtPlayerPosition(transform.gameObject);
        }
        
    }
    public void StartAbsorb()
    {
        isAbsorbingResources = true;
    }
    public void StopAbsorb()
    {
        isAbsorbingResources = false;
    }
    public void StartLabor()
    {
        performingLabor = true;
    }
    public void PerformLabor()
    {   
        if (objectToLabor != null)
        {
            Station stationComp = objectToLabor.GetComponent<Station>();
            if (stationComp != null)
            {   
                stationComp.worker = this; //assign worker to station
                stationComp.executeLabor();
            }
        }
    }
    public void cancelLabor()
    {
        performingLabor = false;
        if (objectToLabor != null)
        {
            Station stationComp = objectToLabor.GetComponent<Station>();
            if (stationComp != null)
            {
                stationComp.cancelLabor();
            }
        }
    }
    public void GrabDrop()
    {
        if (!isCarryingObject)
        {
            GrabObject();
        }
        else
        {
            DropObject();
        }
    }
    public void AbsorbResources()
    {
        Debug.Log("Absorbing Resources");
    }
    private void GrabObject()
    {   
        if(objectToGrab != null)
        {
            isCarryingObject = true;

            // Disable object's physics if necessary
            Rigidbody2D objectRb = objectToGrab.GetComponent<Rigidbody2D>();
            if (objectRb != null)
            {
                objectRb.isKinematic = true;
                objectRb.simulated = false;
                objectRb.velocity = Vector2.zero;
                objectRb.angularVelocity = 0f;
            }

            // Calculate the local offset between the object and the carry position
            grabOffset = grabArea.transform.InverseTransformPoint(objectToGrab.transform.position);

            // Parent the object to the carry position
            objectToGrab.transform.SetParent(grabArea.transform);
            

            if (objectToGrab != null)
            {
                // Parent the object to the carry position
                objectToGrab.transform.SetParent(grabArea.transform);
                //objectToGrab.transform.localPosition = grabOffset;
                //objectToGrab.transform.localRotation = Quaternion.identity;
            }
        }
        
    }
    private void DropObject()
    {
        isCarryingObject = false;

        // Re-enable object's physics if necessary
        Rigidbody2D objectRb = objectToGrab.GetComponent<Rigidbody2D>();
        if (objectRb != null)
        {
            objectRb.isKinematic = false;
            objectRb.simulated = true;
        }

        if (objectToGrab != null)
        {
            // Unparent the object
            objectToGrab.transform.SetParent(null);
        }
    }

    public void releaseAbsorbedObjects()
    {
        foreach (GameObject obj in listobjectsToGrab)
        {
            obj.transform.SetParent(null);
            Rigidbody2D objectRb = obj.GetComponent<Rigidbody2D>();
            if (objectRb != null)
            {
                objectRb.isKinematic = false;
                objectRb.simulated = true;
            }
            obj.GetComponent<dynamicSortingOrder>().invertOrder = false;
        }
        listobjectsToGrab.Clear();
    }

    public void sortObsorbedObject(GameObject obj)
    {
        //obj.transform.parent = transform;
        //obj.transform.position = transform.position;
        if(listobjectsToGrab.Count >= maxObjectsToCarry)
        {
            return;
        }
        listobjectsToGrab.Add(obj);

        Rigidbody2D objectRb = obj.GetComponent<Rigidbody2D>();
        if (objectRb != null)
        {
            objectRb.isKinematic = true;
            objectRb.simulated = false;
            objectRb.velocity = Vector2.zero;
            objectRb.angularVelocity = 0f;
        }

        // Calculate the local offset between the object and the carry position
        //grabOffset = grabArea.transform.InverseTransformPoint(obj.transform.position);
        int index = listobjectsToGrab.Count-1;
        // Parent the object to the carry position
        obj.transform.SetParent(grabArea.transform);
        obj.transform.localPosition = new Vector3(0, index / 2.0f * 1.5f, 0);// grabArea.transform.position;// + new Vector3(0, index/2.0f *1.5f, 0);
        obj.GetComponent<dynamicSortingOrder>().invertOrder = true;
        //obj.transform.localPosition = transform.position + new Vector3(0,index,0);
    }
    
    public GameObject GetObjectToGrab()
    {   
        if (isCarryingObject)
        {
            return objectToGrab;
        } else
        {
            return null;
        }
        
    }
    public void DropAndDestroy()
    {
        if (isCarryingObject)
        {
            isCarryingObject = false;
            objectToGrab.transform.SetParent(null);
            Destroy(objectToGrab);
        }
        
    }

    IEnumerator ScaleOverTime(GameObject obj, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;
        coroutineRunning = true;
        while (elapsed < duration)
        {
            // Normalize the elapsed time
            float t = elapsed / duration;

            // Apply the easing function
            float scaleValue = Tween.EaseOutBack(t);

            // Interpolate the scale
            obj.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set
        obj.transform.localScale = endScale;
        coroutineRunning = false;
    }
    



}