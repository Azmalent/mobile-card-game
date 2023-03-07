using Mirror;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    [TargetRpc]
    public void RpcSetMapSeed(NetworkConnectionToClient conn, int seed)
    {
        var game = SpiralRunner.SpiralRunner.get;
        game.RestartWithSeed(seed);
        Debug.Log("Set seed to " + seed);
    }
}
