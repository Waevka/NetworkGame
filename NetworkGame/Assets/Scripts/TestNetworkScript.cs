using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestNetworkScript : MonoBehaviour {

    int reliableChannelId;
    int maxConnections;
    int socketId;
    int socketPort;
    int connectionId;

	// Use this for initialization
	void Start () {
        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        maxConnections = 3;
        HostTopology hostTopology = new HostTopology(connectionConfig, maxConnections);
        socketPort = 8888;
        socketId = NetworkTransport.AddHost(hostTopology, socketPort);
        Debug.Log("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ButtonConnect()
    {
        byte error;
        connectionId = NetworkTransport.Connect(socketId, "localhost", socketPort, 0, out error);
        if((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("ooops");
            return;
        } else
        {
            Debug.Log("Connected to host");
        }
    }

    private void OnDestroy()
    {
        NetworkTransport.Shutdown();
    }
}
