using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabRegion : MonoBehaviour
{
    private playerController pController;
    public bool debug = false;
    // Start is called before the first frame update
    void Start()
    {
        pController = transform.parent.GetComponent <playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D  other)
    {
        if (debug) Debug.Log("In Trigger with: " + other);
        if (pController.isCarryingObject != true)
        //if(pController.listobjectsToGrab.Count < pController.maxObjectsToCarry)
        {
            if(TagUtilities.HasTag(other.gameObject, TagType.Grabbable))
            {
                if (debug) Debug.Log("GRABABBLE OBJECT DEFINED: " + other);
                pController.objectToGrab = other.gameObject; //STORE GRABABBLE OBJECT
                pController.listobjectsToGrab.Add(other.gameObject); //add to list of objects to grab
            }
            if (TagUtilities.HasTag(other.gameObject, TagType.Workable))
            {
                if (debug) Debug.Log("LABOR OBJECT DEFINED: " + other);
                pController.objectToLabor = other.gameObject; //STORE LABOR OBJECT
                //other.gameObject.GetComponent<Station>().worker = pController;
                other.gameObject.GetComponent<Station>().isBeingInspected = true;
                other.gameObject.GetComponent<Station>().doUpdate = true;
            }
            if (TagUtilities.HasTag(other.gameObject, TagType.Inspectable))
            {
                if (debug) Debug.Log("INSPECTABLE OBJECT DEFINED");
                pController.objectToInspect = other.gameObject; //STORE INSPECTABLE OBJECT
                pController.doUpdate = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (debug) Debug.Log("Trigger edit with: " + other);
        if (pController.isCarryingObject != true)
        //if (pController.listobjectsToGrab.Count < pController.maxObjectsToCarry)
        {
            if (TagUtilities.HasTag(other.gameObject, TagType.Grabbable))
            {
                if (debug) Debug.Log("GRABABBLE OBJECT RELEASED");
                pController.objectToGrab = null; //RELEASE GRABABBLE OBJECT
                pController.listobjectsToGrab.Remove(other.gameObject); //remove to list of objects to grab
            }
            if (TagUtilities.HasTag(other.gameObject, TagType.Workable))
            {
                if (debug) Debug.Log("LABOR OBJECT RELEASED");
                pController.cancelLabor();
                pController.objectToLabor = null; //RELEASE LABOR OBJECT
                other.gameObject.GetComponent<Station>().isBeingInspected = false;

            }
            if (TagUtilities.HasTag(other.gameObject, TagType.Inspectable))
            {
                if (debug) Debug.Log("INSPECTABLE OBJECT RELEASED");
                pController.objectToInspect = null; //RELEASE INSPECTABLE OBJECT
                
            }
        }
    }

}
