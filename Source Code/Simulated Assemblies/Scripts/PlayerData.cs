using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private playersInfo playerManager;
    
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.FindAnyObjectByType<playersInfo>();
        //if (!playerManager.allPlayers.Contains(gameObject)) playerManager.allPlayers.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
