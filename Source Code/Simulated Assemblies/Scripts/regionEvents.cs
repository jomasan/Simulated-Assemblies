using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Add this line

public class regionEvents : MonoBehaviour
{
    // Messages to display when the player enters or exits
    public string enterMessage = "Player entered the area.";
    public string exitMessage = "Player left the area.";
    
    // Add UnityEvents for enter and exit
    //public UnityEvent onPlayerEnter;
    //public UnityEvent onPlayerExit;

    public int regionID = 0;
    public creationManager cManager;

    private void Start()
    {
        cManager = GameObject.FindObjectOfType<creationManager>();
    }

    // Called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (TagUtilities.HasTag(other.gameObject, TagType.Player))
        {
            Debug.Log(enterMessage);
            //onPlayerEnter.Invoke(); // Invoke the enter event
            int playerID = other.GetComponent<playerController>().playerID;
            cManager.SetSpriteToPlayer(playerID, regionID);
        }

        /*
        if (other.CompareTag("Player"))
        {
            Debug.Log(enterMessage);
            //onPlayerEnter.Invoke(); // Invoke the enter event
            int playerID = other.GetComponent<playerController>().playerID;
            cManager.SetSpriteToPlayer(playerID, regionID);
        }
        */
    }
       
    // Called when another collider exits the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(exitMessage);
            //onPlayerExit.Invoke(); // Invoke the exit event
        }
    }
}
