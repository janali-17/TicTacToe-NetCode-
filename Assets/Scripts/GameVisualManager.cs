using Mono.Cecil;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 2.0f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform tickPrefab;
    [SerializeField] private Transform lineCompletePrefab;


    private void Start()
    {
        GameManger.Instance.OnGridPos += GameManger_OnGridPos;
        GameManger.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManger.OnGameWinEventArgs e)
    {
        float eulerZ = 0f;
        switch (e.line.orientation)
        {
            default:
            case GameManger.Orientation.Horizontal: eulerZ = 0f; break;
            case GameManger.Orientation.Vertical: eulerZ = 90f; break;
            case GameManger.Orientation.DiagonalA: eulerZ = 46f; break;
            case GameManger.Orientation.DiagonalB: eulerZ = -45f; break;
        }

        Transform lineCompleteTransform = Instantiate(lineCompletePrefab, GetGridWorldPos(e.line.centerGridPos.x, e.line.centerGridPos.y), Quaternion.Euler(0,0,eulerZ));
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    private void GameManger_OnGridPos(object sender, GameManger.OnGridPosEventArg e)
    {
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManger.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManger.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManger.PlayerType.Tick:
                prefab = tickPrefab;
                break;
        }

        Transform spawnCrossTransform = Instantiate(prefab, GetGridWorldPos(x, y), Quaternion.identity);
        spawnCrossTransform.GetComponent<NetworkObject>().Spawn(true);

    }

    private Vector2 GetGridWorldPos(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
