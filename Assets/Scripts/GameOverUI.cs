using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Button onRematchButton;

    private void Awake()
    {
        onRematchButton.onClick.AddListener(() =>
        {
            GameManger.Instance.RematchRpc();
        });
    }

    private void Start()
    {
        GameManger.Instance.OnGameWin += GameManager_OnGameWin;
        GameManger.Instance.OnRematch += GameManager_OnRematch;
        Hide();

    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
       Hide();
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
