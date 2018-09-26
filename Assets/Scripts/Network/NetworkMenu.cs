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
    public float DiscoveryUpdatePeriod = 0.5f;
    private float _timeToRefreshMatch = 0;
    public Dropdown networkMatchesDropwork;
    private List<NetworkBroadcastResult> _matches = new List<NetworkBroadcastResult>();
    private List<Dropdown.OptionData> _optionMatchesList = new List<Dropdown.OptionData>();
    private Dictionary<string, string> matchesData = new Dictionary<string, string>();
    private LocalController _localController;


    void Start()
    {
        _localController = NetworkConfigurationGetter.getConfigurationObject();
        AddListeners();
        NetworkManagerSpecific.Discovery.Initialize();


    }

    private void OnClientConnect(NetworkConnection conn)
    {
        LoadGameScene();
    }

    private void AddListeners()
    {
        StartLanButton.onClick.AddListener(CreateLanMatch);
        ConnectButton.onClick.AddListener(OnClientConnectClicked);
    }



    public void LoadGameScene()
    {
        NetworkManagerSpecific.singleton.ServerChangeScene("WhoStartSceneNetwork");
    }

    private void Update()
    {
        if (!_isConnected && _localController.NetworkType == "LAN")
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
            //LoadGameScene();
        }
      

        
    }

    private void RefreshMatches()
    {
        // filter matches
        Debug.Log("Estou aqui");
        _matches.Clear();
        _optionMatchesList.Clear();

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

}
