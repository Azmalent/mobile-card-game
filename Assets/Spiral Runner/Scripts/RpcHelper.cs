using Mirror;
using UnityEngine;

using SR = SpiralRunner;


public class RpcHelper : NetworkBehaviour
{
    private static RpcHelper m_instance;

    public static RpcHelper get {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<RpcHelper>();
                if (m_instance == null)
                    Debug.LogError("Can't find" + typeof(RpcHelper) + "!");
            }
            return m_instance;
        }
    }

    public static bool HasInstance {
        get { return m_instance != null; }
    }

    //[TargetRpc]
    //public void RestartClientWithHostMap(NetworkConnectionToClient conn)
    //{
    //    //var game = SpiralRunner.SpiralRunner.get;
    //    //int seed = game.GameController.MapView.seed;

    //    //Debug.Log($"RestartClientWithHostMap: seed={seed}");

    //    //game.RestartClientWithSeed(seed);
    //}

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
