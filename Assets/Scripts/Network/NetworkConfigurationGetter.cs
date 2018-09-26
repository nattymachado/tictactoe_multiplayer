using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkConfigurationGetter {


    private static string _networkConfigurationZoneName = "ConfigurationNetworkZone";


    public static LocalController getConfigurationObject()
    {
        Debug.Log("Inicializei configuração 1");
        Scene configurationZone = SceneManager.GetSceneByName(_networkConfigurationZoneName);
        GameObject configurationObject = configurationZone.GetRootGameObjects()[0];
        LocalController localConfiguration = configurationObject.GetComponent<LocalController>();
        Debug.Log("Inicializei configuração");
        return localConfiguration;
    }


}
