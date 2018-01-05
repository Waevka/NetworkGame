using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControlled : Player {
    string message = "";
	// Use this for initialization
	void Start () {
        PlayerManager.Instance.AddNewPlayer(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (!TestNetworkScript.Instance.IsServer)
        {
            float xPos = Input.GetAxis("Horizontal") * 0.2f;
            float yPos = Input.GetAxis("Vertical") * 0.2f;
            transform.Translate(xPos, 0.0f, yPos);
            message = playerName + " pos " + transform.position.x + " " + transform.position.y + " " + transform.position.z;
            TestNetworkScript.Instance.SendNetworkMessageToServer(message);
        }
	}

    public override void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
