using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class TestNetworkScript : MonoBehaviour {

    private static TestNetworkScript instance;
    public static TestNetworkScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TestNetworkScript>();
            }
            return instance;
        }
    }

    int reliableChannelId;
    int maxConnections;
    int socketId;
    int socketPort;
    int connectionId;
    [SerializeField]
    private MessageParser parser;
    [SerializeField]
    GameObject serverButton;
    [SerializeField]
    GameObject clientButton;
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject playerControlledPrefab;
    Dictionary<int, GameObject> connectionList;
    [SerializeField]
    GameStateUpdater gameStateUpdater = null;

    public bool Initialized = false;
    public bool IsServer = false;
    public bool bothServerAndClient = false;
    public bool isListening = false;

	// Use this for initialization
	void Start () { 
        NetworkTransport.Init();
        connectionList = new Dictionary<int, GameObject>();
        parser = GetComponent<MessageParser>();
        isListening = true;
        StartCoroutine(MessageReceiver());
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private IEnumerator MessageReceiver()
    {
        while (isListening)
        {
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
                    if (IsServer)
                    {
                        InitializeNewPlayer(IsServer, recConnectionId);
                    }
                    InfoMenu.Instance.WriteLine("Somebody connected");
                    Debug.Log("Somebody connected!");
                    break;
                case NetworkEventType.DataEvent:
                    Stream stream = new MemoryStream(recBuffer);
                    BinaryFormatter formatter = new BinaryFormatter();
                    string message = formatter.Deserialize(stream) as string;
                    //Debug.Log("Incoming message: " + message);
                    parser.ParseMessage(message, recConnectionId);
                    break;
                case NetworkEventType.DisconnectEvent:
                    if (IsServer)
                    {
                        RemoveClient(recConnectionId);
                    }
                    InfoMenu.Instance.WriteLine("Somebody disconnected");
                    Debug.Log("Somebody disconnected!");
                    break;
            }
            yield return null;
        }

    }

    public bool ConnectToServer(string ipAddress, int portNumber)
    {
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        maxConnections = 3;
        HostTopology hostTopology = new HostTopology(connectionConfig, maxConnections);

        socketPort = portNumber - 1;
        int retryCount = 0;
        while (retryCount < 3)
        {
            //Up to 3 retries/3 connections
            socketId = NetworkTransport.AddHost(hostTopology, socketPort);
            InfoMenu.Instance.WriteLine("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);
            Debug.Log("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);
            byte error;
            connectionId = NetworkTransport.Connect(socketId, ipAddress, portNumber, 0, out error);
            if ((NetworkError)error != NetworkError.Ok) //If connection failed
            {
                Debug.Log("ooops - " + (NetworkError)error + ", try no. " + retryCount);
                Debug.Log("Odpauzuj jesli wyskoczy blad :P Albo odznacz pause on error");
                retryCount++;
                socketPort--;
            }
            else // If connected successfully
            {
                Initialized = true;
                return true;
                //InitializePlayer();
                //break;
            }
        }
        return false;
    }

    public void InitializePlayer()
    {
        if (IsServer)
        {
            bothServerAndClient = true;
        }
        //Initialized = true;

        Debug.Log("Connected to host");
        InfoMenu.Instance.WriteLine("Connected to host");
        if (!bothServerAndClient)
        {
            InitializeNewPlayer(false, 0);
        }
        else
        {
            InitializeServerClient();
        }

    }

    public bool LogInToServer(string login, string pass)
    {
        InfoMenu.Instance.WriteLine("Trying to log in user: " + login);
        return MainMenu.Instance.IsUserInDatabase(login, pass);
    }

    public void ButtonCreateServer()
    {
        gameStateUpdater = gameObject.GetComponent<GameStateUpdater>();
        if(gameStateUpdater == null)
        {
            gameStateUpdater = gameObject.AddComponent<GameStateUpdater>();
        }
        gameStateUpdater.enabled = true;
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        maxConnections = 3;
        HostTopology hostTopology = new HostTopology(connectionConfig, maxConnections);
        socketPort = 8889;
        socketId = NetworkTransport.AddHost(hostTopology, socketPort);
        InfoMenu.Instance.WriteLine("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);
        Debug.Log("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);

        /*byte error;
        connectionId = NetworkTransport.Connect(socketId, "127.0.0.1", socketPort, 0, out error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("ooops - " + (NetworkError)error);
            return;
        }
        else
        {
            Initialized = true;
            Debug.Log("Connected to host");
        }*/
#if !UNITY_EDITOR
        if(clientButton != null)
        {
            clientButton.SetActive(false);
        }
#endif
        Initialized = true;
        IsServer = true;
        parser.IsServer = true;
    }

    private void OnDestroy()
    {
        NetworkTransport.Shutdown();
    }

    public bool SendNetworkMessageToServer(string message)
    {
        if (!Initialized) return false; 

        byte[] bytemessage = new byte[1240];
        Stream stream = new MemoryStream(bytemessage);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        byte error;
        NetworkTransport.Send(socketId, connectionId, reliableChannelId, bytemessage, 1024, out error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("Error sending message: "+ message + " " + (NetworkError)error);
            return false;
        }
        else
        {
            Debug.Log("Sent message: " + message);
            return true;
        }
    }


    public bool SendNetworkMessageToClient(string message, int connectionId)
    {
        if (!Initialized) return false;

        byte[] bytemessage = new byte[1240];
        Stream stream = new MemoryStream(bytemessage);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        byte error;
        NetworkTransport.Send(socketId, connectionId, reliableChannelId, bytemessage, 1024, out error);
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("Error sending message: " + message + " " + (NetworkError)error);
            return false;
        }
        else
        {
            Debug.Log("Sent message: " + message);
            return true;
        }
    }

    public void SendNetworkMessageToAllClients(string message)
    {
        StartCoroutine(ClientBroadcaster(message));
    }

    public void SendNetworkMessageToAllOtherClients(string message, int connectionId)
    {
        StartCoroutine(ClientBroadcaster(message, connectionId));
    }

    private void InitializeNewPlayer(bool initAsServer, int connectionId)
    {
        if (connectionList.ContainsKey(connectionId) || bothServerAndClient) return;
        if (initAsServer)
        {
            string defaultPos;
            GameObject p = Instantiate(playerPrefab);
            connectionList.Add(connectionId, p);
            Player player = p.GetComponent<Player>();
            player.playerName = "PlayerID" + connectionId;
            PlayerManager.Instance.AddNewPlayer(player);
            defaultPos = player.GetPositionString();
            SendNetworkMessageToClient("servermsg setname " + p.name, connectionId);
            SendNetworkMessageToClient("servermsg " + defaultPos, connectionId);
            SendNetworkMessageToAllOtherClients(
                "servermsg crpl " + defaultPos,
                connectionId);
            SendNetworkMessageToAllOtherClients("servermsg hl " + player.health, connectionId);
            StartCoroutine(SendInitialData(connectionId));
        } else
        {
            GameObject p = Instantiate(playerControlledPrefab);
            //getInitialData();
        }
    }

    private void RemoveClient(int clientId)
    {
        GameObject quitter;
        connectionList.TryGetValue(clientId, out quitter);
        if(quitter != null)
        {
            PlayerManager.Instance.RemovePlayer(
                PlayerManager.Instance.GetPlayer(quitter.GetComponent<Player>().playerName)
                );
        }
        SendNetworkMessageToAllOtherClients("servermsg dlpl " + quitter.GetComponent<Player>().playerName, connectionId);
        connectionList.Remove(clientId);
    }

    private void InitializeServerClient()
    {
        GameObject p = Instantiate(playerControlledPrefab);
        p.GetComponent<PlayerControlled>().playerName = "ServerClient";
        PlayerManager.Instance.AssignMyName("ServerClient");
    }

    private IEnumerator SendInitialData(int connectionId)
    {
       foreach(int connection in connectionList.Keys)
        {
            if(connection != connectionId)
            {
                GameObject gp;
                connectionList.TryGetValue(connection, out gp);
                Player p = gp.GetComponent<Player>();
                string playerdata = "servermsg crpl " + p.GetPositionString();
                SendNetworkMessageToClient(playerdata, connectionId);
            }
            yield return null;
        }
    }

    private IEnumerator ClientBroadcaster(string message, int connectionId = -1)
    {
        foreach(int connection in connectionList.Keys)
        {
            if(connection != connectionId)
            SendNetworkMessageToClient(message, connection);
            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        isListening = false;
    }

    public Dictionary<int, GameObject> GetConnectionList()
    {
        return connectionList;
    }
}
