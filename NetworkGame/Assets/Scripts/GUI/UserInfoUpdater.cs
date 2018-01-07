using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoUpdater : MonoBehaviour {
    [SerializeField]
    Image HpBar;
    [SerializeField]
    Text Name;
	// Use this for initialization
	void Start () {
        Name.text = transform.parent.gameObject.GetComponent<Player>().playerName;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
        var currentRot = transform.rotation.eulerAngles;
        currentRot.x = 145.0f;
        transform.rotation = Quaternion.Euler(currentRot);
        if(HpBar != null) HpBar.fillAmount = (float)PlayerManager.Instance.MyPlayer.health / 100.0f;
    }
}
