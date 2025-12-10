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

    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayerTypeChanged;
    public enum PlayerType
    {
        None,
        Cross,
        Tick,
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayableType = new NetworkVariable<PlayerType>();

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
        if (playerType != currentPlayableType.Value)
        {
            return;
        }
        OnGridPos?.Invoke(this, new OnGridPosEventArg
        {
            x = x,
            y = y,
            playerType = playerType,
        });

        switch (currentPlayableType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayableType.Value = PlayerType.Tick;
                break;
            case PlayerType.Tick:
                currentPlayableType.Value = PlayerType.Cross;
                break;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this,EventArgs.Empty);
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
            currentPlayableType.Value = PlayerType.Cross;

            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayableType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayerTypeChanged?.Invoke(this,EventArgs.Empty);
        };
    }



    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayableType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }


    public PlayerType GetPlayerType() { return localPlayerType; }

    public PlayerType GetCurrentPlayerType () { return currentPlayableType.Value; }
}
