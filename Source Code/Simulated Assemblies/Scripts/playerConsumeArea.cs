using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerConsumeArea : MonoBehaviour
{
    private playerController pController;
    public bool debug = true;
    void Start()
    {
        pController = transform.parent.GetComponent<playerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (debug) Debug.Log("In Trigger with: " + other);

        if (TagUtilities.HasTag(other.gameObject, TagType.Resource) && TagUtilities.HasTag(other.gameObject, TagType.Consumable))
        {
            if (debug) Debug.Log("Consumed: " + other);
            Destroy(other.gameObject);
            pController.capital += 1;
        }else if(pController.isAbsorbingResources && TagUtilities.HasTag(other.gameObject, TagType.Resource) && TagUtilities.HasTag(other.gameObject, TagType.Grabbable))
        {
            if (debug) Debug.Log("Absorbed: " + other);
            
            pController.sortObsorbedObject(other.gameObject);
            
        }

    }
}
