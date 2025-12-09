using System;
using Unity.Netcode;
using UnityEngine;

public class GameManger : NetworkBehaviour
{
    public static GameManger Instance { get; private set; }

    public event EventHandler<OnGridPosEventArg> OnGridPos;
    public class OnGridPosEventArg : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Tick,
    }

    private PlayerType localPlayerType;
    private PlayerType currentPlayableType;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Already have Game Manager Instance");
        }
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void OnGridClickedRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("Grid Pos Clicked" + x + ", " + y);
        if (playerType != currentPlayableType)
        {
            return;
        }
        OnGridPos?.Invoke(this, new OnGridPosEventArg
        {
            x = x,
            y = y,
            playerType = playerType,
        });

        switch (currentPlayableType)
        {
            default:
                case PlayerType.Cross:
                currentPlayableType = PlayerType.Tick;
                break;
                case PlayerType.Tick:
                currentPlayableType = PlayerType.Cross;
                break;
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Tick;
        }

        if (IsServer)
        {
            currentPlayableType = PlayerType.Cross;
        }
    }

    public PlayerType GetPlayerType() { return localPlayerType; }
}
