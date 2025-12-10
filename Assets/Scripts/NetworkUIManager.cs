using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private Button onStartHost;
    [SerializeField] private Button onStartClient;


    private void Awake()
    {
        onStartHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        onStartClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
