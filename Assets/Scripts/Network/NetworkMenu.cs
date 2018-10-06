using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using System.Text;

public class NetworkMenu : MonoBehaviour
{


    private string _roomName = "";
    private bool _isConnected = false;
    public Button StartLanButton;
    public Button ConnectButton;
    public Button StartLobbyButton;
    public float DiscoveryUpdatePeriod = 0.5f;
    private float _timeToRefreshMatch = 0;
    public Dropdown networkMatchesDropwork;
    private List<NetworkBroadcastResult> _matches = new List<NetworkBroadcastResult>();
    private List<Dropdown.OptionData> _optionMatchesList = new List<Dropdown.OptionData>();
    private Dictionary<string, string> matchesData = new Dictionary<string, string>();
    private BoardNetworkConfiguration _configuration;


    void Start()
    {
       AddListeners();
        
        _configuration = NetworkConfigurationGetter.getConfigurationObject();

     


    }

    private void OnClientConnect(NetworkConnection conn)
    {
        LoadGameScene();
    }

    private void AddListeners()
    {
        StartLanButton.onClick.AddListener(CreateLanMatch);
        ConnectButton.onClick.AddListener(OnClientConnectClicked);
        StartLobbyButton.onClick.AddListener(CreateLobbyMatch);
    }



    public void LoadGameScene()
    {
        NetworkManagerSpecific.singleton.ServerChangeScene("WhoStartSceneNetwork");
    }

    private void Update()
    {
        if (!_isConnected )
        {
            _timeToRefreshMatch -= Time.deltaTime;
            if (_timeToRefreshMatch < 0)
            {
                RefreshMatches();

                _timeToRefreshMatch = DiscoveryUpdatePeriod;
            }
        }
    }

    private void OnClientConnectClicked()
    {
        if (networkMatchesDropwork.value > -1)
        {
            string serverAddress = matchesData[networkMatchesDropwork.options[networkMatchesDropwork.value].text];
            NetworkManagerSpecific.singleton.networkAddress = serverAddress;
            NetworkManagerSpecific.singleton.StartClient();

            NetworkManagerSpecific.Discovery.StopBroadcast();
            _isConnected = true;
        }
      

        
    }

    private void RefreshMatches()
    {
        // filter matches
        _matches.Clear();
        _optionMatchesList.Clear();
        if (NetworkManagerSpecific.Discovery)
        {
            foreach (var match in NetworkManagerSpecific.Discovery.broadcastsReceived.Values)
            {
                Debug.Log(match.broadcastData);
                var matchId = Encoding.Unicode.GetString(match.broadcastData);
                Debug.Log(matchId);
                if (matchesData.ContainsKey(matchId))
                {
                    continue;
                }

                _optionMatchesList.Add(new Dropdown.OptionData(matchId));

                matchesData[matchId] = match.serverAddress;
            }
        }
        
        networkMatchesDropwork.AddOptions(_optionMatchesList);

       
    }

    public void CreateLanMatch()
    {
        NetworkManagerSpecific.Discovery.StopBroadcast();

        _roomName = "Natalia";
        NetworkManagerSpecific.Discovery.broadcastData = "Natalia";
        NetworkManagerSpecific.Discovery.StartAsServer();

        NetworkManagerSpecific.singleton.StartHost();

        Debug.Log("Estou conectada");
        _isConnected = true;
    }

    public void CreateLobbyMatch()
    {
        //NetworkLobbyManagerSpecific.LobbyManager.MMCreateMatch();
        NetworkLobbyManagerSpecific.LobbyManager.MMListMaches();
    }

}
