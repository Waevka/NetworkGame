﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName;
    public int health = 100;
	// Use this for initialization
	void Start () {
        //PlayerManager.Instance.AddNewPlayer(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    public string GetPositionString()
    {
        return playerName + " pos " + transform.position.x + " " + transform.position.y + " " + transform.position.z;
    }

    public string GetRotationString()
    {
        Vector3 rotvec = transform.rotation.eulerAngles;
        return playerName + " rot " + rotvec.x + " " + rotvec.y + " " + rotvec.z;
    }

    public void SetRotation(string x, string y, string z)
    {
        transform.rotation = Quaternion.Euler(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
