using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageParser : MonoBehaviour {
    public bool IsServer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ParseMessage(string m, int connectionID)
    {
        Player p = null;
        string[] splitMessage = m.Split();
        if(splitMessage[0] != null)
        {
            if(splitMessage[0] == "servermsg")
            {
                Debug.Log(m);
                ParseServerMsg(splitMessage);
            } else
            {
                if (splitMessage[0] == "login")
                {
                    if (IsServer)
                    {
                        if (TestNetworkScript.Instance.LogInToServer(splitMessage[1], splitMessage[2]))
                        {
                            string message = "servermsg login 1 " + splitMessage[1];
                            TestNetworkScript.Instance.SendNetworkMessageToClient(message, connectionID);
                            TestNetworkScript.Instance.InitializeNewPlayer(IsServer, connectionID, splitMessage[1]);
                        }
                        else
                        {
                            string message = "servermsg login 0 " + splitMessage[1];
                            TestNetworkScript.Instance.SendNetworkMessageToClient(message, connectionID);
                        }
                        return;
                    }
                }

                p = PlayerManager.Instance.GetPlayer(splitMessage[0]);
                if (p == null) return;
                switch (splitMessage[1])
                {
                    case "pos":
                        p.SetPosition(splitMessage[2], splitMessage[3], splitMessage[4]);
                        if (IsServer) TestNetworkScript.Instance.SendNetworkMessageToAllOtherClients(p.GetPositionString(), connectionID);
                        break;
                    case "rot":
                        p.SetRotation(splitMessage[2], splitMessage[3], splitMessage[4]);
                        if (IsServer) TestNetworkScript.Instance.SendNetworkMessageToAllOtherClients(p.GetRotationString(), connectionID);
                        break;
                    case "at1": //attack1
                        p.PlayAnimation("SwordSlash");
                        if (IsServer) TestNetworkScript.Instance.SendNetworkMessageToAllOtherClients("servermsg at1 " + p.playerName, connectionID);
                        break;
                    default:
                        break;
                }

            }
        }
    }

    public void ParseServerMsg(string[] msg)
    {
        switch (msg[1])
        {
            case "setname": //asigns a name
                PlayerManager.Instance.AssignMyName(msg[2]);
                break;
            case "pos": //this will override client's position if he's messed up
                break;
            case "crpl": //createplayer
                PlayerManager.Instance.CreateNewPlayer(msg);
                break;
            case "dlpl": //deleteplayer
                PlayerManager.Instance.DeletePlayer(msg);
                break;
            case "hl":
                PlayerManager.Instance.SetHealth(msg);
                break;
            case "login":
                MainMenu.Instance.LogInResponse(msg);
                break;
            case "at1":
                PlayerManager.Instance.PlayPlayerAnimation(msg);
                break;
            default:
                break;

        }
    }
}
