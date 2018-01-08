using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerCharacter")
        {
            if (TestNetworkScript.Instance.IsServer || other.gameObject.GetComponent<PlayerControlled>())
            {
                PlayerManager.Instance.AddSecretPoint(other.gameObject.name, 1);
            }
            Destroy(gameObject);
        }
    }
}
