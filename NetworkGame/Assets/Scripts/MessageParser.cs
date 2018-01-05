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
