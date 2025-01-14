using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playersInfo : MonoBehaviour
{
    public List <GameObject> playerInfoPanels = new List<GameObject>();
    public List <GameObject> allPlayers;

    public List <Color> playerColors = new List<Color>();

    public List <string> playerNames = new List<string>();

    public List<playerController> allControllers = new List<playerController>();
    List<playerInfoManager> allInfoManagers = new List<playerInfoManager>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerInfoPanels.Count; i++)
        {
            playerInfoManager playerinfo = playerInfoPanels[i].GetComponent<playerInfoManager>();
            allInfoManagers.Add(playerinfo);
            if (playerNames[i] != null) playerinfo.playerName.text = playerNames[i];
        }

        /*
        for (int i = 0; i < allControllers.Count; i++)
        {
            playerController pc = allPlayers[i].GetComponent<playerController>();
            allControllers.Add(pc);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < allControllers.Count; i++)
        {
            allInfoManagers[i].capital.text = "$" + allControllers[i].capital.ToString() ;
        }
    }
}
