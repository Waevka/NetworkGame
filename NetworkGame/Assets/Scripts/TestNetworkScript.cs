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
    int unreliableChannelId;
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
    [SerializeField]
    GameObject pickupPrefab;
    Dictionary<int, GameObject> connectionList;
    [SerializeField]
    GameStateUpdater gameStateUpdater = null;

    public bool Initialized = false;
    public bool IsServer = false;
    public bool bothServerAndClient = false;
    public bool isListening = false;

	/// <summary>
    /// Initializes TestNetworkScript object.
    /// Invokes Init() on Unity low level API NetworkTransport.
    /// Starts coroutine that receives network messages from server or client.
    /// </summary>
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

    /// <summary>
    /// Reveives network messages of type NetworkEventType in a loop.
    /// Supported events:
    /// - ConnectEvent
    /// - DisconnectEvent
    /// - DataEvent
    /// - nothing received
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Tries to estabilish a connection to server, using provided IP address and port number.
    /// Default IP is 127.0.0.1 and default port is 8888.
    /// In case of connection failure, up to 3 retries are executed.
    /// Sets the "Initialized" flag on success.
    /// </summary>
    /// <param name="ipAddress">IP address entered by user</param>
    /// <param name="portNumber">Port number entered by user</param>
    /// <returns></returns>
    public bool ConnectToServer(string ipAddress, int portNumber)
    {
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        unreliableChannelId = connectionConfig.AddChannel(QosType.UnreliableSequenced);
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
            Debug.Log("New connection id: " + connectionId);
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

    /// <summary>
    /// Initializes a player object.
    /// Server side - initializes a simple Player object used to store data.
    /// Client side - initializes a controllable PlayerControlled object.
    /// </summary>
    /// <param name="username">Username of player that is being initialized</param>
    public void InitializePlayer(string username)
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
            InitializeNewPlayer(false, 0, username);
        }
        else
        {
            InitializeServerClient();
        }

    }

    /// <summary>
    /// Executed on Connect button click by client.
    /// </summary>
    /// <param name="login">Login/name of user.</param>
    /// <param name="pass">User password.</param>
    /// <returns>If login has been successful or not.</returns>
    public bool LogInToServer(string login, string pass)
    {
        InfoMenu.Instance.WriteLine("Trying to log in user: " + login);
        return MainMenu.Instance.IsUserInDatabase(login, pass);
    }

    /// <summary>
    /// Creates a listening server by initializing further parameters of
    /// TransportNetwork API.
    /// </summary>
    /// <param name="portNumber">Port number for the server.</param>
    public void CreateServer(int portNumber)
    {
        gameStateUpdater = gameObject.GetComponent<GameStateUpdater>();
        if (gameStateUpdater == null)
        {
            gameStateUpdater = gameObject.AddComponent<GameStateUpdater>();
        }
        gameStateUpdater.enabled = true;
        gameStateUpdater.SetPickupPrefab(pickupPrefab);
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        unreliableChannelId = connectionConfig.AddChannel(QosType.UnreliableSequenced);
        maxConnections = 3;
        HostTopology hostTopology = new HostTopology(connectionConfig, maxConnections);
        socketPort = portNumber;
        socketId = NetworkTransport.AddHost(hostTopology, socketPort);
        InfoMenu.Instance.WriteLine("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);
        Debug.Log("Started NetworkTransport host on port " + socketPort + ", socketID is " + socketId);

        Initialized = true;
        IsServer = true;
        parser.IsServer = true;
    }

    /// <summary>
    /// Creates a listening server by initializing further parameters of
    /// TransportNetwork API. Executes on button press by server.
    /// </summary>
    public void ButtonCreateServer()
    {
        gameStateUpdater = gameObject.GetComponent<GameStateUpdater>();
        if(gameStateUpdater == null)
        {
            gameStateUpdater = gameObject.AddComponent<GameStateUpdater>();
        }
        gameStateUpdater.enabled = true;
        gameStateUpdater.SetPickupPrefab(pickupPrefab);
        ConnectionConfig connectionConfig = new ConnectionConfig();
        reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
        unreliableChannelId = connectionConfig.AddChannel(QosType.UnreliableSequenced);
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

    /// <summary>
    /// Shuts down NetworkTransport when game window is closed.
    /// </summary>
    private void OnDestroy()
    {
        NetworkTransport.Shutdown();
    }

    /// <summary>
    /// Sends a message from client to server.
    /// </summary>
    /// <param name="message">Server message</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    /// <returns>If the sending succeeded or not.</returns>
    public bool SendNetworkMessageToServer(string message, bool reliable)
    {
        if (!Initialized) return false; 

        byte[] bytemessage = new byte[1024];
        Stream stream = new MemoryStream(bytemessage);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        byte error;
        int channelId = (reliable ? reliableChannelId : unreliableChannelId);
        Debug.Log("trying to send to id " + connectionId);
        NetworkTransport.Send(socketId, connectionId, channelId, bytemessage, 1024, out error);
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

    /// <summary>
    /// Sends a network message to a single client.
    /// </summary>
    /// <param name="message">Server message</param>
    /// <param name="connectionId">Connection ID of recipent client.</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    /// <returns>If the sending succeeded or not.</returns>
    public bool SendNetworkMessageToClient(string message, int connectionId, bool reliable)
    {
        if (!Initialized) return false;

        byte[] bytemessage = new byte[1024];
        Stream stream = new MemoryStream(bytemessage);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        byte error;
        int channelId = (reliable ? reliableChannelId : unreliableChannelId);
        NetworkTransport.Send(socketId, connectionId, channelId, bytemessage, 1024, out error);
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

    /// <summary>
    /// Finds a client connectionID by client name and then sends a network message.
    /// </summary>
    /// <param name="message">Server message.</param>
    /// <param name="name">Name of the recipent client.</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    /// <returns>If the sending succeeded or not.</returns>
    public bool SendNetworkMessageToClient(string message, string name, bool reliable)
    {
        GameObject p = PlayerManager.Instance.GetPlayer(name).gameObject;
        foreach(KeyValuePair <int, GameObject> pl in connectionList)
        {
            if(pl.Value == p)
            {   
                return SendNetworkMessageToClient(message, pl.Key, reliable);
            }
        }
        return false;
    }

    /// <summary>
    /// Sends a network message to all currently connected clients using a broadcaster coroutine.
    /// </summary>
    /// <param name="message">Server message</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    public void SendNetworkMessageToAllClients(string message, bool reliable)
    {
        StartCoroutine(ClientBroadcaster(message, reliable));
    }

    /// <summary>
    /// Sends a network message to all currently connected clients BUT the provided client.
    /// </summary>
    /// <param name="message">Server message</param>
    /// <param name="connectionId">Connection ID of client that should be excluded.</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    public void SendNetworkMessageToAllOtherClients(string message, int connectionId, bool reliable)
    {
        StartCoroutine(ClientBroadcaster(message, reliable, connectionId));
    }

    /// <summary>
    /// Creates a new player object and saves its' info. Informs all other players
    /// that a new player has connected. Sends current state of the game to the
    /// new client.
    /// </summary>
    /// <param name="initAsServer">If the game client belongs to a server or client.</param>
    /// <param name="connectionId">Connection ID of new player.</param>
    /// <param name="clientName">Client name of new player.</param>
    public void InitializeNewPlayer(bool initAsServer, int connectionId, string clientName)
    {
        if (connectionList.ContainsKey(connectionId) || bothServerAndClient) return;
        if (initAsServer)
        {
            string defaultPos;
            GameObject p = Instantiate(playerPrefab);
            connectionList.Add(connectionId, p);
            Player player = p.GetComponent<Player>();
            player.playerName = clientName;
            PlayerManager.Instance.AddNewPlayer(player);
            defaultPos = player.GetPositionString();
            SendNetworkMessageToClient("servermsg setname " + p.name, connectionId, true);
            SendNetworkMessageToClient("servermsg " + defaultPos, connectionId, true);
            SendNetworkMessageToAllOtherClients(
                "servermsg crpl " + defaultPos,
                connectionId, true);
            SendNetworkMessageToAllOtherClients("servermsg hl " + player.playerName + " 100", connectionId, true);
            SendInitialData(connectionId);
        } else
        {
            GameObject p = Instantiate(playerControlledPrefab);
            //getInitialData();
        }
    }

    /// <summary>
    /// Removes a client from connection list. Might be due to a disconnect
    /// or the client was marked as suspicious (cheating).
    /// </summary>
    /// <param name="clientId">Client ID of removed client.</param>
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
        SendNetworkMessageToAllOtherClients("servermsg dlpl " + quitter.GetComponent<Player>().playerName, connectionId, true);
        connectionList.Remove(clientId);
    }

    /// <summary>
    /// Special case for a Server instance that also spawns a PlayerControlled.
    /// To be used in development.
    /// </summary>
    private void InitializeServerClient()
    {
        GameObject p = Instantiate(playerControlledPrefab);
        p.GetComponent<PlayerControlled>().playerName = "ServerClient";
        PlayerManager.Instance.AssignMyName("ServerClient");
    }

    /// <summary>
    /// Sends current game state to a new player, using a non blocking coroutine.
    /// </summary>
    /// <param name="connectionId">Connection ID of new player.</param>
    public void SendInitialData(int connectionId)
    {
        StartCoroutine(SendInitialDataRoutine(connectionId));
    }

    /// <summary>
    /// Non blocking coroutine for sending initial game state to a new player.
    /// </summary>
    /// <param name="connectionId">Connection ID of new player.</param>
    /// <returns></returns>
    private IEnumerator SendInitialDataRoutine(int connectionId)
    {
       foreach(int connection in connectionList.Keys)
        {
            if(connection != connectionId)
            {
                GameObject gp;
                connectionList.TryGetValue(connection, out gp);
                Player p = gp.GetComponent<Player>();
                string playerdata = "servermsg crpl " + p.GetPositionString();
                SendNetworkMessageToClient(playerdata, connectionId, true);
                playerdata = p.GetRotationString();
                SendNetworkMessageToClient(playerdata, connectionId, true);
            }
            yield return null;
        }
        string[] pickupData = gameStateUpdater.GetAllMuffins();
        foreach(string s in pickupData)
        {
            SendNetworkMessageToClient(s, connectionId, true);
            yield return null;
        }
    }

    /// <summary>
    /// Broadcasts a message to all connected players. Can be used to send
    /// a message to all client BUT one provided.
    /// </summary>
    /// <param name="message">Server message.</param>
    /// <param name="reliable">Defines if message will be sent as reliable packet or not.</param>
    /// <param name="connectionId">Does not have to be provided.</param>
    /// <returns></returns>
    private IEnumerator ClientBroadcaster(string message, bool reliable, int connectionId = -1)
    {
        foreach(int connection in connectionList.Keys)
        {
            if(connection != connectionId)
            SendNetworkMessageToClient(message, connection, reliable);
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

    /// <summary>
    /// Kicks a player that has been marked as suspicious.
    /// </summary>
    /// <param name="p"></param>
    public void KickPlayer(Player p)
    {
        foreach (KeyValuePair<int, GameObject> player in connectionList)
        {
            if (player.Value == p.gameObject)
            {
                byte error;
                NetworkTransport.Disconnect(socketId, player.Key, out error);
                if ((NetworkError)error != NetworkError.Ok)
                {
                    Debug.Log("Error kicking player: " + p.playerName + " " + (NetworkError)error);
                }
                RemoveClient(player.Key);
                break;
            }
        }
    }
}
