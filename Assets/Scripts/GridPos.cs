using UnityEngine;

public class GridPos : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    private void OnMouseDown()
    {
        Debug.Log("Click " + x +","+ y);
        GameManger.Instance.OnGridClickedRpc(x, y,GameManger.Instance.GetPlayerType());
    }
}
