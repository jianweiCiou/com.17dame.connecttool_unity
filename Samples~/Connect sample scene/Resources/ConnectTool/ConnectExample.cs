using DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectExample : MonoBehaviour
{
    public static ConnectExample Instance { get; private set; }

    ConnectTool _connectTool;

    [SerializeField] private Button RegisterButton = null;
    [SerializeField] private Button getConnectAuthorizeButton = null;
    [SerializeField] private Button LoginButton = null;
    [SerializeField] private Button postConnectTokenButton = null;
    [SerializeField] private Button getMeButton = null;
    [SerializeField] private Button postConnectRefreshTokenButton = null; 
     
    // Start is called before the first frame update
    void Start()
    {
        _connectTool = new ConnectTool(
            "",
            "",
            "",
            "");

        _connectTool.connectBasic = new ConnectBasic()
        {
            client_id = "",
            X_Developer_Id = "",
            client_secret = "",
            Game_id = "",
            referralCode = ""
        }; 
         
        RegisterButton.onClick.AddListener(() => { PostRegisterData(); });
        getConnectAuthorizeButton.onClick.AddListener(() => { _connectTool.OpenAuthorizeURL(); });
        LoginButton.onClick.AddListener(() => { PostLoginData(); });
        postConnectTokenButton.onClick.AddListener(() => { PostConnectTokenData(); });
        postConnectRefreshTokenButton.onClick.AddListener(() => { PostRefreshTokenData(); });
        getMeButton.onClick.AddListener(() => { GetConnect_Me(); }); 
    }



    void PostRegisterData()
    {
        _connectTool.CreateAccountInitData(
           "",
         ""); 

        _connectTool.SendRegisterData((code) =>
        {
            Debug.Log(code);
            if (code)
            {
                sendToast("Register success");
            }
        });
    }

    void PostLoginData()
    {
        _connectTool.CreateAccountInitData(
           "",
         ""); 

        _connectTool.SendLoginData((_result) =>
        {
            Debug.Log(_result);
            sendToast(_result.ToString());
        });
    }

    void PostConnectTokenData()
    {
        StartCoroutine(_connectTool.GetConnectToken_Coroutine((_tokenResponse) =>
        {
            Debug.Log(JsonUtility.ToJson(_tokenResponse).ToString());
            sendToast(JsonUtility.ToJson(_tokenResponse).ToString());
        }));
    }

    void PostRefreshTokenData()
    {
        StartCoroutine(_connectTool.GetRefreshToken_Coroutine((_tokenResponse) =>
        {
            Debug.Log(JsonUtility.ToJson(_tokenResponse).ToString());
            sendToast(JsonUtility.ToJson(_tokenResponse).ToString());
        }));
    }

    void GetConnect_Me()
    {
        StartCoroutine(_connectTool.GetMe_Coroutine((_me) =>
        {
            Debug.Log(JsonUtility.ToJson(_me).ToString());
            sendToast(JsonUtility.ToJson(_me).ToString());
        }));
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                onDeepLinkActivated(Application.absoluteURL);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void onDeepLinkActivated(string url)
    {
        Uri codeUri = new Uri(url);
        string code = HttpUtility.ParseQueryString(codeUri.Query).Get("code");
        _connectTool.code = code;

        bool validScene = false;
        string sceneName = url.Split('?')[1];
        if (sceneName.Contains("connectscene"))
        {
            validScene = true;
        }
        if (validScene) SceneManager.LoadScene("connectscene");
    }

    public static void sendToast(string message)
    {
        AndroidJavaClass ajc = new AndroidJavaClass("com.r17dame.connectplugin.UnityToast");
        ajc.CallStatic("ShowToast", message);
    }
}
