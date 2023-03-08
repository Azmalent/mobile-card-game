using Mirror;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    [TargetRpc]
    public void RestartClientWithHostMap(NetworkConnectionToClient conn)
    {
        var game = SpiralRunner.SpiralRunner.get;
        int seed = game.GameController.MapView.seed;

        Debug.Log($"RestartClientWithHostMap: seed={seed}");

        game.RestartClientWithSeed(seed);
    }
}
