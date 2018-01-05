using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageParser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ParseMessage(string m)
    {
        Player p = null;
        string[] splitMessage = m.Split();
        if(splitMessage[0] != null)
        {
            if(splitMessage[0] == "servermsg")
            {
                ParseServerMsg(splitMessage);
            } else
            {
                p = PlayerManager.Instance.GetPlayer(splitMessage[0]);
                if (p == null) return;
                switch (splitMessage[1])
                {
                    case "pos":
                        p.SetPosition(splitMessage[2], splitMessage[3], splitMessage[4]);
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
            case "assignname":
                PlayerManager.Instance.AssignMyName(msg[2]);
                break;
            default:
                break;

        }
    }
}
