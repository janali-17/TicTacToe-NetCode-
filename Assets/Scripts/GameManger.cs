using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnRematch;
    public event EventHandler OnGameTie;

    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }

    public struct Line
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPos;
        public Orientation orientation;
    }
    public enum PlayerType
    {
        None,
        Cross,
        Tick,
    }
    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayableType = new NetworkVariable<PlayerType>();
    private PlayerType[,] playerTypeArray;
    private List<Line> lineList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Already have Game Manager Instance");
        }
        Instance = this;

        playerTypeArray = new PlayerType[3, 3];

        lineList = new List<Line>
        {
            // Horizontal
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), },
                centerGridPos = new Vector2Int(1,0),
                orientation = Orientation.Horizontal,
            },
              new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1), },
                centerGridPos = new Vector2Int(1,1),
                orientation = Orientation.Horizontal,

            },
                new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(2,2), },
                centerGridPos = new Vector2Int(1,2),
                orientation = Orientation.Horizontal,
            },
            // Vertical
                new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2), },
                centerGridPos = new Vector2Int(0,1),
                orientation = Orientation.Vertical,

            },
                       new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(1,2), },
                centerGridPos = new Vector2Int(1,1),
                orientation = Orientation.Vertical,
            },
                              new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(2,2), },
                centerGridPos = new Vector2Int(2,1),
                orientation = Orientation.Vertical,
            },
            // Diagonal
                   new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,2), },
                centerGridPos = new Vector2Int(1,1),
                orientation = Orientation.DiagonalA,
            },
                          new Line
            {
                gridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,0), },
                centerGridPos = new Vector2Int(1,1),
                orientation = Orientation.DiagonalB,

            },

        };
    }

    [Rpc(SendTo.Server)]
    public void OnGridClickedRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("Grid Pos Clicked" + x + ", " + y);

        if (playerType != currentPlayableType.Value)
        {
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None)
        {
            return;
        }

        playerTypeArray[x, y] = playerType;

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
        CheckWinner();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
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
            OnCurrentPlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };
    }


    private bool TestWinnerline(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return
            aPlayerType != PlayerType.None &&
            aPlayerType == bPlayerType &&
            bPlayerType == cPlayerType;
    }

    private bool TestWinnerLine(Line line)
    {
        return TestWinnerline(
            playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
            );
    }

    private void CheckWinner()
    {
        for (int i = 0; i < lineList.Count; i++)
        {
            Line line = lineList[i];
            if (TestWinnerLine(line))
            {
                currentPlayableType.Value = PlayerType.None;
                TriggerOnGameWinRpc(i, playerTypeArray[line.centerGridPos.x, line.centerGridPos.y]);
                break;
            }
        }

        bool hasTie = true;
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                if (playerTypeArray[x, y] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }
            }
        }
        if (hasTie)
        {
            TriggerOnGameTieRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTieRpc()
    {
        OnGameTie?.Invoke(this,EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                playerTypeArray[x,y] = PlayerType.None;
            }
        }
        currentPlayableType.Value = PlayerType.Cross;

        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
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

    public PlayerType GetCurrentPlayerType() { return currentPlayableType.Value; }
}
