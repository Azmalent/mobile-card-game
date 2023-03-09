using Mirror;
using Mirror.Discovery;
using UnityEngine;

using SR = SpiralRunner;

public class SpiralRunnerNetworkManager : NetworkManager {
    public static ServerResponse? serverInfo { get; private set; } = null;

    private NetworkDiscovery networkDiscovery;

    private GameObject m_firstPlayer;
    private GameObject m_secondPlayer;

    private NetworkConnectionToClient m_firstConnection;
    private NetworkConnectionToClient m_secondConnection;

    public override void Start() {
        base.Start();

        networkDiscovery = GetComponent<NetworkDiscovery>();

        DiGro.Check.NotNull(networkDiscovery);

        networkDiscovery.StartDiscovery();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerPrefab);

        if (numPlayers == 0) {
            player.name = $"Player [host, connId={conn.connectionId}]";
            m_firstPlayer = player;
            m_firstConnection = conn;
        }
        if (numPlayers == 1) {
            player.name = $"Player [client, connId={conn.connectionId}]";
            m_secondPlayer = player;
            m_secondConnection = conn;
        }

        NetworkServer.AddPlayerForConnection(conn, player);

        if (m_firstPlayer != null && m_secondPlayer != null) 
            InitOnlineGame();
    }

    private void InitOnlineGame() {
        int mapSeed = SR.SpiralRunner.get.GameController.MapView.seed;

        RpcHelper.get.TargetSetPlayers(m_firstConnection, m_firstPlayer, m_secondPlayer, m_firstPlayer.name, m_secondPlayer.name);
        RpcHelper.get.TargetSetPlayers(m_secondConnection, m_secondPlayer, m_firstPlayer, m_secondPlayer.name, m_firstPlayer.name);

        RpcHelper.get.TargetRecreateMapWithSeed(m_secondConnection, mapSeed);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
    }

    public void OnDiscoveredServer(ServerResponse info) {
        networkDiscovery.StopDiscovery();
        serverInfo = info;
    }

    public void CreateGame() {
        Debug.Log("Creating game...");

        var game = SR.SpiralRunner.get;

        game.IsNetworkGame = true;
        game.GameController.RemoveSinglePlayer();

        StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void JoinGame() {
        Debug.Log("Joining game...");

        var game = SR.SpiralRunner.get;

        game.IsNetworkGame = true;
        game.GameController.RemoveSinglePlayer();

        StartClient(serverInfo.Value.uri);
    }
}

//public struct CreateCharacterMessage : NetworkMessage { }

//public class SpiralRunnerNetworkManager : NetworkManager
//{
//    [SerializeField] private RpcHandler rpc;

//    private int m_connectedPlayerCount = 0;

//    public static ServerResponse? serverInfo { get; private set; } = null;
//    private NetworkDiscovery networkDiscovery;

//    public override void Start() 
//    {
//        base.Start();
//        networkDiscovery = GetComponent<NetworkDiscovery>(); 
//        networkDiscovery.StartDiscovery();   
//    }

//    public override void OnStartServer() {
//        base.OnStartServer();

//        //NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
//    }

//    public override void OnServerConnect(NetworkConnectionToClient conn) {
//        base.OnServerConnect(conn);

//        Debug.Log($"OnServerConnect: connectionId = {conn.connectionId}");
//    }

//    public override void OnServerReady(NetworkConnectionToClient conn) {
//        base.OnServerReady(conn);

//        Debug.Log($"OnServerReady: connectionId = {conn.connectionId}");
//    }

//    public override void OnClientConnect() {
//        base.OnClientConnect();

//        var conn = NetworkClient.connection;

//        Debug.Log($"OnClientConnect: connectionId = {conn.connectionId}");

//        //NetworkClient.Send(new CreateCharacterMessage());
//    }


//    //public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
//    //    Debug.Log($"OnServerAddPlayer: connectionId = {conn.connectionId}");

//    //    var player = Instantiate(playerPrefab);
//    //    //float angle = numPlayers == 0 ? 0 : 45;
//    //    float pos = numPlayers == 0 ? 0 : 4.5f;

//    //    player.transform.position = new Vector3(pos, 0, 0);
//    //    //player.transform.localRotation = Quaternion.Euler(0, angle, 0);

//    //    //player.name = $"Player [connId={conn.connectionId}]";
//    //    NetworkServer.AddPlayerForConnection(conn, player);
//    //}

//    //void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message) {
//    //    Debug.Log($"OnCreateCharacter: connectionId = {conn.connectionId}");

//    //    var game = SR.SpiralRunner.get;

//    //    m_connectedPlayerCount++;

//    //    GameObject player = null;

//    //    if (m_connectedPlayerCount == 1) {
//    //        player = game.GameController.LocalPlayer.gameObject;
//    //        player.name = $"PlayerController [host, connId={conn.connectionId}]";
//    //    }
//    //    if (m_connectedPlayerCount == 2) {
//    //        player = game.ConnectPlayerToHost();
//    //        player.name = $"PlayerController [client, connId={conn.connectionId}]";

//    //        rpc.RestartClientWithHostMap(conn);
//    //    }

//    //    NetworkServer.AddPlayerForConnection(conn, player);
//    //}

//    public override void OnServerDisconnect(NetworkConnectionToClient conn)
//    {
//        base.OnServerDisconnect(conn);
//    }

//    public void OnDiscoveredServer(ServerResponse info)
//    {
//        networkDiscovery.StopDiscovery();
//        serverInfo = info;
//    }

//    public new void StartHost()
//    {
//        SR.SpiralRunner.get.IsNetworkGame = true;
//        Debug.Log("Hosting game...");

//        base.StartHost();
//        networkDiscovery.AdvertiseServer();
//    }

//    public void JoinHost()
//    {
//        SR.SpiralRunner.get.IsNetworkGame = true;
//        Debug.Log("Joining game...");

//        StartClient(serverInfo.Value.uri);
//    }
//}
