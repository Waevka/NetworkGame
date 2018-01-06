using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControlled : Player {
    string message = "";
    public bool isInitialized = false;
	// Use this for initialization
	void Start () {
        //PlayerManager.Instance.AddNewPlayer(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isInitialized) return;
        float xPos = Input.GetAxis("Horizontal") * 0.2f;
        float yPos = Input.GetAxis("Vertical") * 0.2f;
        if(yPos != 0 || xPos != 0)
        {
            transform.Translate(xPos, 0.0f, yPos);
            message = GetPositionString();
            if (!TestNetworkScript.Instance.IsServer)
            {
                TestNetworkScript.Instance.SendNetworkMessageToServer(message);
            }
        }
    }

    public override void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
