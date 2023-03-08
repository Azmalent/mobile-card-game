using Mirror;
using Mirror.Discovery;
using UnityEngine;

using SR = SpiralRunner;

public class SpiralRunnerNetworkManager : NetworkManager {
    public static ServerResponse? serverInfo { get; private set; } = null;


    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerPrefab);

        player.name = $"Player [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
    }

    public new void StartHost() { }

    public void JoinHost() { }
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
