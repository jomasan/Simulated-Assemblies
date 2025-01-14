using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creationManager : MonoBehaviour
{
    [Header("References")]
    public GameObject [] sprites;               // Array of sprites to choose from
    public int spriteID = 0;               // Current sprite ID
    
    // Dictionary to map player IDs to sprite IDs
    private Dictionary<int, int> playerSpriteMap = new Dictionary<int, int>();
    
    // Start is called before the first frame update
    void Start()
    {
        playerSpriteMap.Add(0,0);
        playerSpriteMap.Add(1,0);
        playerSpriteMap.Add(2,0);
        playerSpriteMap.Add(3,0);
        playerSpriteMap.Add(4,0);
        playerSpriteMap.Add(5,0);
        playerSpriteMap.Add(6,0);
        playerSpriteMap.Add(7,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            CreateSpriteAtMousePosition();
        }
    }
    
    public void CreateSpriteAtMousePosition()
    {
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned in the SpriteManager.");
            return;
        }

        if (spriteID < 0 || spriteID >= sprites.Length)
        {
            Debug.LogWarning("Sprite ID is out of range.");
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            // Instantiate the sprite prefab as a child of the canvas
            GameObject newSprite = Instantiate(sprites[spriteID], hit.point, Quaternion.identity);
        }
    }

    public void CreateSpriteAtPlayerPosition(GameObject player)
    {
        int playerID = player.GetComponent<playerController>().playerID;
        int spriteIDforPlayer = GetSpriteIdForPlayer(playerID);
        
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned in the SpriteManager.");
            return;
        }

        if (spriteID < 0 || spriteID >= sprites.Length)
        {
            Debug.LogWarning("Sprite ID is out of range.");
            return;
        }
        
        GameObject newSprite = Instantiate(sprites[spriteIDforPlayer], player.transform.position, Quaternion.identity);
    }
    
    // Method to update the sprite ID from the inventory system
    public void SetSpriteID(int newID)
    {
        spriteID = newID;
    }
    
    // Method to assign a sprite ID to a player
    public void SetSpriteToPlayer(int playerId, int spriteId)
    {
        if (playerSpriteMap.ContainsKey(playerId))
        {
            playerSpriteMap[playerId] = spriteId;
            Debug.Log("Player: " + playerId + " assigned sprite ID: " + spriteId);
        }
        else
        {
            playerSpriteMap.Add(playerId, spriteId);
        }
    }
    
    public int GetSpriteIdForPlayer(int playerId)
    {
        if (playerSpriteMap.TryGetValue(playerId, out int spriteId))
        {
            return spriteId;
        }
        else
        {
            Debug.LogWarning($"No sprite ID found for player ID {playerId}");
            return -1; // Or any default/error value
        }
    }
    
    
}
