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
        //base.Start();
        //networkDiscovery = GetComponent<NetworkDiscovery>(); 
        //networkDiscovery.StartDiscovery();   
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Transform startPos = GetStartPosition();
        //     GameObject player = startPos != null
        //         ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
        //         : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        var gameController = SpiralRunner.SpiralRunner.get.GameController;
        //var player = gameController.SpawnPlayer(numPlayers - 1);
        //player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        //NetworkServer.AddPlayerForConnection(conn, player.gameObject);

        if (numPlayers == 2)
        {
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
    }

    public void JoinHost()
    {
        StartClient(serverInfo.Value.uri);
    }
}
