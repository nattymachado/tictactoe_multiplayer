﻿using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using System.Text;

public class NetworkMenu : MonoBehaviour
{


    private bool _isConnected = false;
    public Button StartLanButton;
    public Button ConnectButton;
    public float DiscoveryUpdatePeriod = 0.5f;
    public InputField RoomNameInput;
    private float _timeToRefreshMatch = 0;
    public Dropdown networkMatchesDropwork;
    private List<NetworkBroadcastResult> _matches = new List<NetworkBroadcastResult>();
    private List<Dropdown.OptionData> _optionMatchesList = new List<Dropdown.OptionData>();
    private Dictionary<string, string> matchesData = new Dictionary<string, string>();
    private BoardNetworkConfiguration _configuration;
    private BoardConfiguration _configurationGame;


    void Start()
    {
        _configuration = NetworkConfigurationGetter.getConfigurationObject();
        _configurationGame = BoardConfigurationGetter.getConfigurationObject();
        AddListeners();
    }

    private void OnClientConnect(NetworkConnection conn)
    {
        LoadGameScene();
    }

    private void AddListeners()
    {
        if (_configurationGame.Network == NetworkOptions.Options.Lan)
        {
            StartLanButton.onClick.AddListener(CreateLanMatch);
            ConnectButton.onClick.AddListener(OnClientConnectClicked);
        } else
        {
           StartLanButton.onClick.AddListener(CreateLobbyMatch);
        }
    }



    public void LoadGameScene()
    {
        NetworkManagerSpecific.singleton.ServerChangeScene("WhoStartSceneNetwork");
    }

    private void Update()
    {
       if (!_isConnected)
            {
                _timeToRefreshMatch -= Time.deltaTime;
                if (_timeToRefreshMatch < 0) { 

                    if (_configurationGame && _configurationGame.Network == NetworkOptions.Options.Lan) {
                        RefreshMatches();
                    } else
                    {
                        ListLobbyMatches();
                    }

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
        NetworkManagerSpecific.Discovery.broadcastData = RoomNameInput.text;
        NetworkManagerSpecific.Discovery.StartAsServer();
        NetworkManagerSpecific.singleton.StartHost();
        _isConnected = true;
    }

    public void CreateLobbyMatch()
    {
        if (NetworkLobbyManagerSpecific.LobbyManager)
        {
            NetworkLobbyManagerSpecific.LobbyManager.MMCreateMatch(RoomNameInput.text);

        }
    }

    public void ListLobbyMatches()
    {
        if (NetworkLobbyManagerSpecific.LobbyManager)
        {
            NetworkLobbyManagerSpecific.LobbyManager.MMListMaches(this.networkMatchesDropwork);
        }
        
    }

}
