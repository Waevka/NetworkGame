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
    private Text MyIdText;
    [SerializeField]
    GameObject otherPlayerPrefab;
    [SerializeField]
    GameObject userInfoPanelPrefab;

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
        Instantiate(userInfoPanelPrefab, p.gameObject.transform, false);
        Debug.Log("Added new player: " + p.playerName);
        InfoMenu.Instance.WriteLine("Added new player: " + p.playerName);
    }

    public void RemovePlayer(Player p)
    {
        playerList.Remove(p.playerName);
        Destroy(p.gameObject);
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
        if(MyIdText != null) MyIdText.text = name;
    }

    public void CreateNewPlayer(string[] msg)
    {
        Player p = Instantiate(otherPlayerPrefab).GetComponent<Player>();
        p.playerName = msg[2];
        p.SetPosition(msg[4], msg[5], msg[6]);
        AddNewPlayer(p);
    }

    public void DeletePlayer(string[] msg)
    {
        Player p = GetPlayer(msg[2]);
        if(p != null)
        {
            RemovePlayer(p);
        }
    }

    public void SetHealth(string[] msg)
    {
        Player p = GetPlayer(msg[1]);
        if (p != null)
        {
            p.health = 100;
        }
    }

    public string[] GetAllPlayerPositions()
    {
        string[] allpos = new string[playerList.Count];
        int idx = 0;
        foreach(Player p in playerList.Values)
        {
            allpos[idx] = p.GetPositionString();
            idx++;
        }
        return allpos;
    }

    public string[] GetAllPlayerRotations()
    {
        string[] allrot = new string[playerList.Count];
        int idx = 0;
        foreach (Player p in playerList.Values)
        {
            allrot[idx] = p.GetRotationString();
            idx++;
        }
        return allrot;
    }
}
