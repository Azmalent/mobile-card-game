using Mirror;
using UnityEngine;

using SR = SpiralRunner;


public class RpcHelper : DiGro.SingletonMirror<RpcHelper>
{
    [TargetRpc]
    public void TargetSetPlayers(NetworkConnection target, GameObject local, GameObject remote, string name1, string name2) {
        var game = SR.SpiralRunner.get;

        local.name = name1;
        remote.name = name2;

        var localPlayer = local.GetComponent<SR.Controller.PlayerController>();
        var remotePlayer = remote.GetComponent<SR.Controller.PlayerController>();

        DiGro.Check.NotNull(localPlayer);
        DiGro.Check.NotNull(remotePlayer);

        game.GameController.SetOnlinePlayers(localPlayer, remotePlayer);
    }

    [TargetRpc]
    public void TargetRecreateMapWithSeed(NetworkConnection target, int seed) {
        var game = SR.SpiralRunner.get;

        game.GameController.RecreateMapWithSeed(seed);
    }
}
