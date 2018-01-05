using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PlayerManager>();
            }
            return instance;
        }
    }

    Dictionary<string, Player> playerList;
    // Use this for initialization
    private void Awake()
    {
        playerList = new Dictionary<string, Player>();
    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddNewPlayer(Player p)
    {
        playerList.Add(p.playerName, p);
    }

    public void RemovePlayer(Player p)
    {
        playerList.Remove(p.playerName);
    }

    public Player GetPlayer(string name)
    {
        Player p;
        playerList.TryGetValue(name, out p);
        return p;
    }
}
