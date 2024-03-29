﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControlled : Player {
    string message = "";
    public bool isInitialized = false;
	// Use this for initialization
	void Start () {
        IsAlive = true;
        //PlayerManager.Instance.AddNewPlayer(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isInitialized || !IsAlive) return;
        float yRot = Input.GetAxis("Horizontal") * 2.0f;
        float xPos = Input.GetAxis("Vertical") * 0.2f;

        if(xPos != 0)
        {
            transform.Translate(Vector3.forward * xPos);
            message = GetPositionString();
            if (!TestNetworkScript.Instance.IsServer)
            {
                TestNetworkScript.Instance.SendNetworkMessageToServer(message, false);
            }
        }

        if(yRot != 0)
        {
            transform.Rotate(Vector3.up, yRot);
            message = GetRotationString();
            if (!TestNetworkScript.Instance.IsServer)
            {
                TestNetworkScript.Instance.SendNetworkMessageToServer(message, false);
            }
        }

        if(Input.GetButtonDown("Fire1"))
        {
            if (TryAttack())
            {
                message = playerName + " at1";
                if (!TestNetworkScript.Instance.IsServer)
                {
                    TestNetworkScript.Instance.SendNetworkMessageToServer(message, true);
                }
            }
        }
    }

    public override void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
