using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;


public class NetworkLobbyManagerSpecific : NetworkLobbyManager {

    private Dropdown networkMatchesDropwork;
    private Dictionary<string, NetworkId > matchesData = new Dictionary<string, string>();
    private List<Dropdown.OptionData> _optionMatchesList = new List<Dropdown.OptionData>();

    public void Start()
    {
        MMStart();
    }
    
    public static NetworkLobbyManagerSpecific LobbyManager
    {
        get
        {
            Debug.Log("Getting");
            if (singleton)
            {
                return singleton.GetComponent<NetworkLobbyManagerSpecific>();
            } else
            {
                return null;
            }
            
        }
    }

    public static NetworkDiscovery Discovery
    {
        get
        {
            Debug.Log("Getting");
            return singleton.GetComponent<NetworkDiscovery>();
        }
    }

    public void MMStart()
    {
        Debug.Log("@ MMStart");
        this.StartMatchMaker();
    }

    public void MMListMaches(Dropdown listMatches)
    {
        Debug.Log("@ MMListMatches");
        if (!networkMatchesDropwork)
        {
            networkMatchesDropwork = listMatches;
        }
        this.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        Debug.Log("@ OnMatchList");
        base.OnMatchList(success, extendedInfo, matchList);

        if (!success)
        {
            Debug.Log("Failed OnMatchList:" + extendedInfo);
           
        } else
        {

            _optionMatchesList.Clear();
            foreach (var match in matchList)
            {
                Debug.Log(match.name);
                if (matchesData.ContainsKey(match.name))
                {
                    continue;
                }

                _optionMatchesList.Add(new Dropdown.OptionData(match.name));

                matchesData[match.name] = match.networkId;
                _optionMatchesList.Add(new Dropdown.OptionData(match.name));
            }

            networkMatchesDropwork.AddOptions(_optionMatchesList);
        }
    }

    //Detect when a client connects to the Server
    public override void OnClientConnect(NetworkConnection conn)
    {

        //Output text to show the connection on the client side
        Debug.Log(ClientScene.reconnectId);
        Debug.Log("Client Side : Client " + conn.connectionId + " Connected!");
        NetworkManagerSpecific.singleton.ServerChangeScene("WhoStartSceneNetwork");



    }

    public override void ServerChangeScene(string newSceneName)
    {
        Debug.Log("Trocando de página");
        SceneManager.LoadScene(newSceneName);
        base.ServerChangeScene(newSceneName);
    }

    //Detect when a client connects to the Server
    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
        //Output text to show the connection on the client side
        Debug.Log(ClientScene.reconnectId);
        Debug.Log("Client Side : Client " + conn.connectionId + " Connected!");
        NetworkManagerSpecific.singleton.ServerChangeScene("WhoStartSceneNetwork");
    }

    //Detect when a client connects to the Server
    public override void OnClientSceneChanged(NetworkConnection conn)
    {

        ClientScene.Ready(conn);
        
        ClientScene.AddPlayer((short) conn.playerControllers.Count);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Connection ID:" + conn.connectionId);
        GameObject player = (GameObject)Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
        if (conn.connectionId > 0)
        {
            player.tag = "OtherPlayer";
            Debug.Log("Changing tag");
        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public void MMJoinMatch(MatchInfoSnapshot match)
    {
        Debug.Log("@ MMJoinMatch");
        this.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0,OnMatchJoined);
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("@ OnMathcJoined");
        base.OnMatchJoined(success, extendedInfo, matchInfo);

        if (!success)
        {
            Debug.Log("Failed OnMatchJoined:" + extendedInfo);
        }
        else
        {
            Debug.Log("Joined:" + matchInfo.networkId);
        }
    }

    public void MMCreateMatch(String rooomName)
    {
        Debug.Log("@ MMCreateMatch");
        this.matchMaker.CreateMatch(rooomName, 15, true, "", "", "", 0, 0, OnMatchCreate);
    }


    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("@ OnMatchCreate");
        base.OnMatchCreate(success, extendedInfo, matchInfo);

        if (!success)
        {
            Debug.Log("Failed to create match");
        } else
        {
            Debug.Log("Match Created");
        }
    }

    public static void StartDiscovery()
    {
        NetworkLobbyManagerSpecific.Discovery.Initialize();
        NetworkLobbyManagerSpecific.Discovery.StartAsClient();
    }

}
