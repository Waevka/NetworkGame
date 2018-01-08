using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuffinCountUpdater : MonoBehaviour {
    [SerializeField]
    GameObject description;
    Text counter;
	// Use this for initialization
	void Start () {
        description.SetActive(true);
        counter = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        counter.text = (PlayerManager.Instance.MyPlayer != null? PlayerManager.Instance.MyPlayer.GetPoint() : 0 ).ToString();
	}
}
