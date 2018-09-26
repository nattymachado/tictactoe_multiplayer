using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class NetworkManagerSpecific : NetworkManager {

    public static event Action<NetworkConnection> onServerConnect;
    public static event Action<NetworkConnection> onClientConnect;

    public static NetworkDiscovery Discovery
    {
        get
        {
            Debug.Log("Getting");
            return singleton.GetComponent<NetworkDiscovery>();
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (conn.address == "localClient")
        {
            return;
        }

        Debug.Log("Client connected! Address: " + conn.address);

        //conn.playerControllers.Count > 1

        if (onServerConnect != null)
        {
            onServerConnect(conn);
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


    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
    }

    public override void ServerChangeScene(string newSceneName)
    {
        Debug.Log("Trocando de página");
        SceneManager.LoadScene(newSceneName);
        base.ServerChangeScene(newSceneName);
    }

    //Detect when a client connects to the Server
    public override void OnClientSceneChanged(NetworkConnection conn)
    {

        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);
    }


    /*
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    

    */

}
