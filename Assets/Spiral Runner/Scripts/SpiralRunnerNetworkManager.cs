using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class SpiralRunnerNetworkManager : NetworkManager
{
    [SerializeField] private RpcHandler rpc;

    public static ServerResponse? serverInfo { get; private set; } = null;
    private NetworkDiscovery networkDiscovery;

    public override void Start() 
    {
        base.Start();
        networkDiscovery = GetComponent<NetworkDiscovery>(); 
        networkDiscovery.StartDiscovery();   
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var gameController = SpiralRunner.SpiralRunner.get.GameController;
        if (NetworkClient.activeHost)
        {
            var hostPlayer = gameController.LocalPlayer.gameObject;
            hostPlayer.name = $"{playerPrefab.name} [host, connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, hostPlayer.gameObject);
        }
        else 
        {
            var player = gameController.SpawnPlayer(1, true);
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);

            rpc.RpcSetMapSeed(conn, gameController.MapView.seed);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        serverInfo = info;
    }

    public new void StartHost()
    {
        base.StartHost();
        networkDiscovery.AdvertiseServer();
        Debug.Log("Hosting game...");
    }

    public void JoinHost()
    {
        StartClient(serverInfo.Value.uri);
        Debug.Log("Joining game...");
    }
}
