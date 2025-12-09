using System;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance {  get; private set; }

    public event EventHandler<OnGridPosEventArg> OnGridPos;
    public class OnGridPosEventArg : EventArgs
    {
        public int x;
        public int y;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Already have Game Manager Instance");
        }
        Instance = this;
    }

    public void OnGridClicked(int x, int y)
    {
        Debug.Log("Grid Pos Clicked" + x + ", " + y);
        OnGridPos?.Invoke(this, new OnGridPosEventArg
        {
            x = x,
            y = y,
        });
    }
}
