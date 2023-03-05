using Mirror;
using Mirror.Discovery;

public class SpiralRunnerNetworkManager : NetworkManager
{  
    private long? serverId = null;
    private NetworkDiscovery networkDiscovery;

    public override void Start() 
    {
        base.Start();
        networkDiscovery = GetComponent<NetworkDiscovery>();    
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        serverId = info.serverId;
        //TODO: enable join button
    }
}
