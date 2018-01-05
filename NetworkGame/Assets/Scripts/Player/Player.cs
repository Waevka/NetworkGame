using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName;
	// Use this for initialization
	void Start () {
        PlayerManager.Instance.AddNewPlayer(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }
}
