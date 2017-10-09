using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        //Check for messages
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;

        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(
            out recHostId, out recConnectionId, out recChannelId,
            recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Somebody connected!");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Debug.Log("Incoming message: " + message);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Somebody disconnected!");
                break;
        }
    }

    public void ButtonConnect()
    {
        byte error;
        connectionId = NetworkTransport.Connect(socketId, "127.0.0.1", socketPort, 0, out error);
        if((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("ooops - " + (NetworkError)error);
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
