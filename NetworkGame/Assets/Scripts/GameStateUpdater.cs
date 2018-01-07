using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateUpdater : MonoBehaviour {
    public bool started;
    private float updateInterval;
    public int totalTimesUpdated;
	// Use this for initialization
	void Start () {
        started = false;
        updateInterval = 2.0f;
        totalTimesUpdated = 0;
        StartCoroutine(WaitForNetworkInitialized());
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

	// Update is called once per frame
	void Update () {
		
	}
}
