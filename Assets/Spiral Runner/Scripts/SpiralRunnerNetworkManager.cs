using Mirror;
using Mirror.Discovery;

public class SpiralRunnerNetworkManager : NetworkManager
{
    public static ServerResponse? serverInfo { get; private set; } = null;
    public NetworkDiscovery networkDiscovery { get; private set; }

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
        //var gameController = SpiralRunner.SpiralRunner.get.GameController;
        //var player = gameController.SpawnPlayer(numPlayers - 1);
        //player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        //NetworkServer.AddPlayerForConnection(conn, player.gameObject);

        //if (numPlayers == 2)
        //{
        //    var seed = gameController.MapView.seed;
        //    RpcSetMapSeed(seed);
        //}
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

    //TODO: move this shit to NetworkBehaviour
    //[ClientRpc(includeOwner = false)]
    //public void RpcSetMapSeed(int seed)
    //{
    //    var game = SpiralRunner.SpiralRunner.get;
    //    game.RestartWithSeed(seed);
    //}
}
