using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateUpdater : MonoBehaviour {
    public bool started;
    private float updateInterval;
    public int totalTimesUpdated;
    [SerializeField]
    GameObject muffinPrefab;
	// Use this for initialization
	void Start () {
        started = false;
        updateInterval = 2.0f;
        totalTimesUpdated = 0;
        StartCoroutine(WaitForNetworkInitialized());
	}

    public void SetPickupPrefab(GameObject p)
    {
        muffinPrefab = p;
        SpawnInitialMuffins();
    }

    private IEnumerator PlayerPositionUpdater()
    {
        while (started)
        {
            string[] allpos = PlayerManager.Instance.GetAllPlayerPositions();
            foreach(string pos in allpos)
            {
                TestNetworkScript.Instance.SendNetworkMessageToAllClients(pos, false);
                yield return null;
            }
            totalTimesUpdated++;
            yield return new WaitForSeconds(updateInterval);
        }
    }
    private IEnumerator PlayerRotationUpdater()
    {
        while (started)
        {
            yield return new WaitForSeconds(updateInterval);
        }
    }
    private IEnumerator WaitForNetworkInitialized()
    {
        while (!TestNetworkScript.Instance.Initialized)
        {
            yield return new WaitForSeconds(0.1f);
        }
        started = true;

        //init
        StartCoroutine(PlayerPositionUpdater());
        StartCoroutine(PlayerRotationUpdater());
    }
    
    private void SpawnInitialMuffins()
    {
        CreateSingleMuffin("3.0", "1.0", "9.0");
        CreateSingleMuffin("-5.0", "1.0", "2.0");
        CreateSingleMuffin("6.0", "1.0", "5.0");
        CreateSingleMuffin("-9.0", "1.0", "-3.0");
    }

    public string[] GetAllMuffins()
    {
        PointPickup[] muffins = FindObjectsOfType<PointPickup>();
        string[] muffinpos = new string[muffins.Length];
        int i = 0;
        foreach(PointPickup p in muffins)
        {
            muffinpos[i] = "servermsg " + p.GetPickupString();
            i++;
        }
        return muffinpos;
    }

    public void CreateSingleMuffin(string x, string y, string z)
    {
        Instantiate(muffinPrefab, new Vector3(float.Parse(x), float.Parse(y), float.Parse(z)), Quaternion.identity);
    }
    
	// Update is called once per frame
	void Update () {
		
	}
}
