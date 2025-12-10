using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossYouGameObject;
    [SerializeField] private GameObject tickYouGameObject;
    [SerializeField] private GameObject crossTurnGameObject;
    [SerializeField] private GameObject tickTurnGameObject;


    private void Awake()
    {
        crossYouGameObject.SetActive(false);
        tickYouGameObject.SetActive(false);
        crossTurnGameObject.SetActive(false);
        tickTurnGameObject.SetActive(false);
    }

    private void Start()
    {
        GameManger.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManger.Instance.OnCurrentPlayerTypeChanged += GameManager_OnCurrentPlayerTypeChanged;
    }

    private void GameManager_OnCurrentPlayerTypeChanged(object sender, System.EventArgs e)
    {
        UpdateTheCurrentTurn();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        Debug.Log("Game Started");
        if(GameManger.Instance.GetPlayerType() == GameManger.PlayerType.Cross)
        {
           crossYouGameObject.SetActive(true);
        }
        else
        {
            tickYouGameObject.SetActive(true) ;
        }

        UpdateTheCurrentTurn();
    }

    private void UpdateTheCurrentTurn()
    {
        if(GameManger.Instance.GetCurrentPlayerType() == GameManger.PlayerType.Cross)
        {
            crossTurnGameObject.SetActive(true);
            tickTurnGameObject.SetActive(false) ;
        }
        else
        {
            crossTurnGameObject.SetActive(false);
            tickTurnGameObject.SetActive(true);
        }
    }
}
