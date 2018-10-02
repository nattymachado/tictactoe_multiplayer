using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CoinSpawer : NetworkBehaviour
{

    public GameObject coinPrefab;
    private SpriteRenderer _coin;
    private float _luckyNumber = 0;
    private float _totalTime = 0;
    public Sprite coinPlayer1 = null;
    private float _initScaleX = 0;
    private int _starter = 0;
    public Sprite coinPlayer2 = null;
    private GameObject _coinObj1;

    [SyncVar(hook = "OnSetCoin")]
    private GameObject _coinObj;

   [SyncVar(hook = "OnSetScale")]
    private Vector3 _scale;

    private void OnSetCoin(GameObject obj)
    {
        this._coinObj = obj;
    }

    private void OnSetScale(Vector3 vet)
    {
        this._coinObj.transform.localScale = vet;
    }

    public override void OnStartServer()
    {
        _coinObj1 = Instantiate(coinPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        NetworkServer.Spawn(_coinObj1);
        _coin = _coinObj1.GetComponent<SpriteRenderer>();
        _initScaleX = _coin.transform.localScale.x;
        _luckyNumber = Random.Range(10f, 20f);
        _coinObj = _coinObj1;
    }

    [ClientRpc]
    public void RpcSetCoin(GameObject obj)
    {
        this._coinObj = obj; // This is just to trigger the call to the OnSetScale while encapsulating.
    }

    [ClientRpc]
    public void RpcSetScale(Vector3 vec)
    {
        this._scale = vec;
        this._coinObj.transform.localScale = vec; // This is just to trigger the call to the OnSetScale while encapsulating.
    }

    [ClientRpc]
    public void RpcSetSprite(int number)
    {
        if (number == 2)
        {
            this._coinObj.GetComponent<SpriteRenderer>().sprite = coinPlayer1;
        } else
        {
            this._coinObj.GetComponent<SpriteRenderer>().sprite = coinPlayer2;
        } // This is just to trigger the call to the OnSetScale while encapsulating.
    }

    public void Update()
    {
        if (!isServer)
            return;

        _totalTime += Time.deltaTime;
        if (NetworkManagerSpecific.singleton.numPlayers == 2)
        {
            if (_luckyNumber > 0)
            {
                // _audioSource.enabled = true;

                float modTime = (_totalTime % 1);
                if (modTime < 0.25)
                {
                    if (_coin.sprite == coinPlayer2)
                    {
                        _luckyNumber -= 1;
                    }

                    RpcSetSprite(1);
                    RpcSetScale(new Vector3(_initScaleX, _coin.transform.localScale.y, 1f));

                }
                else if (modTime >= 0.25 && modTime < 0.50)
                {
                    RpcSetSprite(1);
                    RpcSetScale(new Vector3(_initScaleX / 2f, _coin.transform.localScale.y, 1f));

                }
                else if (modTime >= 0.50 && modTime < 0.75)
                {
                    if (_coin.sprite == coinPlayer1)
                    {
                        _luckyNumber -= 1;
                    }
                    RpcSetSprite(2);
                    RpcSetScale(new Vector3(_initScaleX, _coin.transform.localScale.y, 1f));

                }
                else
                {
                    RpcSetSprite(2);
                    RpcSetScale(new Vector3(_initScaleX / 2f, _coin.transform.localScale.y, 1f));

                }
            }
            else
            {
                // audioSource.enabled = false;
                if (_coin.sprite == coinPlayer1)
                {
                    _starter = 1;
                }
                else
                {
                    _starter = 2;
                }
                BoardNetworkConfiguration  config = NetworkConfigurationGetter.getConfigurationObject();
                config.Starter = _starter;
                NetworkManagerSpecific.singleton.ServerChangeScene("BoardNetworkScene");
            }
        }

        

    }


   

}