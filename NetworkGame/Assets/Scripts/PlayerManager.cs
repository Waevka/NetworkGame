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
    GameObject DeathText;
    [SerializeField]
    GameObject otherPlayerPrefab;
    [SerializeField]
    GameObject userInfoPanelPrefab;
    [SerializeField]
    GameObject swordPrefab;

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
        Instantiate(swordPrefab, p.gameObject.transform, false);
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
        Player p = GetPlayer(msg[2]);
        if (p != null)
        {
            p.health = int.Parse(msg[3]);
        }
        if (p.health == 0)
        {
            p.Die(false);
        }
    }

    public void SetIsAlive(string[] msg)
    {
        Player p = GetPlayer(msg[2]);
        if (p != null)
        {
            bool thisPlayer = (p == MyPlayer);
            if (msg[3] == "True")
            {
                p.Respawn(thisPlayer);
            }
            else
            {
                p.Die(thisPlayer);
            }
        }
    }

    public void DamagePlayer(string name, int dmgValue)
    {
        Player p = GetPlayer(name);
        if (p != null)
        {
            int health = Mathf.Clamp(p.health - dmgValue, 0, 100);
            SetHealth(new string[] { "", "", name, health.ToString() });
        }
    }

    public void AddSecretPoint(string name, int amount)
    {
        Player p = GetPlayer(name);
        if (p != null)
        {
            p.AddPoint(amount);
        }
    }

    public void SetSecretPoint(string[] msg)
    {
        Player p = GetPlayer(msg[2]);
        if (p != null)
        {
            p.SetPoint(int.Parse(msg[3]));
        }
    }

    public void PlayPlayerAnimation(string[] msg)
    {
        Player p = GetPlayer(msg[1]);
        if (p != null)
        {
            switch (msg[2])
            {
                case "at1":
                    p.PlayAnimation("SwordSlash");
                    break;
                default:
                    break;
            }
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
    public void ShowDeathText(bool show)
    {
        if(DeathText != null)
        {
            DeathText.SetActive(show);
        }
    }
}
