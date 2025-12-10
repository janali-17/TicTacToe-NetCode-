using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    private void Start()
    {
        Hide();
        GameManger.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManger.OnGameWinEventArgs e)
    {
        if(e.winPlayerType == GameManger.Instance.GetPlayerType())
        {
            resultTextMesh.text = "You Win!";
            resultTextMesh.color = winColor;
        }
        else
        {
            resultTextMesh.text = "You Lose!";
            resultTextMesh.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
