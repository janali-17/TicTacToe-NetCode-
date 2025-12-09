using Mono.Cecil;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 2.0f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform TickPrefab;


    private void Start()
    {
        GameManger.Instance.OnGridPos += GameManger_OnGridPos;
    }

    private void GameManger_OnGridPos(object sender, GameManger.OnGridPosEventArg e)
    {
      SpawnObjectRpc(e.x, e.y);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y)
    {
        Transform spawnCrossTransform = Instantiate(crossPrefab,GetGridWorldPos(x,y),Quaternion.identity);
        spawnCrossTransform.GetComponent<NetworkObject>().Spawn(true);
    }    

    private Vector2 GetGridWorldPos(int x, int y)
    {
        return new Vector2(-GRID_SIZE +x * GRID_SIZE, -GRID_SIZE +y * GRID_SIZE);
    }
}
