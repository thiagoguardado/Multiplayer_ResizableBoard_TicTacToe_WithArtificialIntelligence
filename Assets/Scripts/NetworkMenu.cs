using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour {

    public NetworkGameSelectionMatch matchPanelPrefab;
    public Transform matchListPanelParent;
    public GameObject popupMenu;
    public InputField matchNameInput;

    public float refreshListTime = 1f;

    private float refreshListTimer = 0f;
    private bool isConnected = false;
    private MyNetworkManager myNetwork;
    private List<NetworkBroadcastResult> _matches = new List<NetworkBroadcastResult>();

    private void Awake()
    {
        myNetwork = NetworkManager.singleton.GetComponent<MyNetworkManager>();
    }

    private void Start()
    {
        StartLookingForMatches();
    }

    private void Update()
    {

        if (!isConnected)
        {
            refreshListTimer -= Time.deltaTime;

            if (refreshListTimer <= 0f)
            {
                RefreshGames();
                refreshListTimer = refreshListTime;
            }

        }
    }

    #region Display
    private void RefreshGames()
    {
        ClearMatchList();

        _matches.Clear();

        foreach (var item in MyNetworkManager.Discovery.broadcastsReceived.Values)
        {
            AddMatchToList(Encoding.Unicode.GetString(item.broadcastData));
        }
    }


    public void CreateNewMatch()
    {
        popupMenu.SetActive(true);
    }


    public void ConfirmNewMatchCreation()
    {
        if (ValidateMatchName(matchNameInput.text))
        {
            popupMenu.SetActive(false);
            StartBroadcastingNewMatch(matchNameInput.text);
        }
    }


    private bool ValidateMatchName(string matchName)
    {
        return true;
    }

    private void AddMatchToList(string matchName)
    {
        NetworkGameSelectionMatch m = Instantiate(matchPanelPrefab, matchListPanelParent);
        m.SetName(matchName);
    }

    private void ClearMatchList()
    {
        for (int i = 1; i < matchListPanelParent.childCount; i++)
        {
            Destroy(matchListPanelParent.GetChild(i).gameObject);
        }
    }

    #endregion

    #region Network
    private void StartLookingForMatches()
    {
        MyNetworkManager.Discovery.Initialize();
        MyNetworkManager.Discovery.StartAsClient();
    }

    private void StartBroadcastingNewMatch(string newMatchName)
    {
        MyNetworkManager.Discovery.StopBroadcast();
        MyNetworkManager.Discovery.broadcastData = newMatchName;
        MyNetworkManager.Discovery.StartAsServer();
        NetworkManager.singleton.StartHost();
        isConnected = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");

    }
    #endregion
}
