using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public PlayerControlled MyPlayer;
    [SerializeField]
    Text MyIdText;

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
        p.gameObject.name = p.playerName;
        playerList.Add(p.playerName, p);
    }

    public void RemovePlayer(Player p)
    {
        playerList.Remove(p.playerName);
        Destroy(p.gameObject, 0.5f);
    }

    public Player GetPlayer(string name)
    {
        Player p;
        playerList.TryGetValue(name, out p);
        return p;
    }

    public void AssignMyName(string name)
    {
        StartCoroutine(MyObjectFinder(name));
    }

    private IEnumerator MyObjectFinder(string name)
    {
        PlayerControlled me = null;
        while(me == null)
        {
            me = FindObjectOfType<PlayerControlled>();
            yield return null;
        }
        me.playerName = name;
        AddNewPlayer(me);
        me.isInitialized = true;
        MyPlayer = me;
        MyIdText.text = name;
    }
}
