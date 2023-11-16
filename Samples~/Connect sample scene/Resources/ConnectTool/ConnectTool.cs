using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text; 
using System.Collections.Generic;
using UnityEngine.Events;
using System.Net.Http;
using System.Linq;
using DataModel; 

public class ConnectTool
{
    public static ConnectTool Instance { get; private set; }

    // init data
    public string state = "";
    public string requestNumber = "";
    public  string redirect_uri = "";
    public string RSAxmlStr = "";

    // basic info
    public ConnectBasic connectBasic; 

    // REQUEST 
    public AccountInitRequest accountRequestData;

    // User
    public MeInfo me; 
     
    public ConnectToken tokenData;
      
    public static string host = "https://gamar18portal.azurewebsites.net";
    public static string game_api_host = "https://r18gameapi.azurewebsites.net"; 

    // Connect Token
    public string code;   
    public string access_token = ""; 
    public static string refresh_token = "";
     
    // API Me 
    public string GameOrderNo = "";

    public int eCoin = 0;
    public int rebate = 0;
     
    public ConnectTool(string _state, string _requestNumber, string _redirect_uri, string _RSAxmlStr)
    {
        // init 
        state = _state;
        requestNumber = _requestNumber;
        redirect_uri = _redirect_uri;
        RSAxmlStr = _RSAxmlStr; 
    } 

    public string getAccess_token()
    {
        return access_token;
    } 

    public async void SendRegisterData(UnityAction<bool> callbackOnFinish)
    {
        HttpClient client = new HttpClient();   

        var formContent_Register = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email",accountRequestData.email),
            new KeyValuePair<string, string>("password", accountRequestData.password),
            new KeyValuePair<string, string>("referralCode", connectBasic.referralCode),
            new KeyValuePair<string, string>("gameId", connectBasic.Game_id ),
        });

        //²£¥ÍA-Signature 
        var ts = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString(); 
        var msg = accountRequestData.email + ts  ;
        SHA256Managed sha256 = new SHA256Managed();
        var asignBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(msg));
        string asignHex = string.Join("", asignBytes.Select(x => x.ToString("X2")));

        // Header 
        client.DefaultRequestHeaders.Add("ts", ts);
        client.DefaultRequestHeaders.Add("A-Signature", asignHex);

        var response = await client.PostAsync($"{host}/api/account/Register", formContent_Register);
        callbackOnFinish(response.IsSuccessStatusCode);
    }


    public async void SendLoginData(UnityAction<bool> callbackOnFinish)
    {
        HttpClient client = new HttpClient();

        var formContent_login = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", accountRequestData.email),
            new KeyValuePair<string, string>("password",  accountRequestData.password),
            new KeyValuePair<string, string>("referralCode", connectBasic.referralCode),
        });
        var ts = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
        var msg = accountRequestData.email + ts;
        SHA256Managed sha256 = new SHA256Managed();

        byte[] asignBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(msg));
        string asignHex = string.Join("", asignBytes.Select(x => x.ToString("X2")));

        // Header 
        client.DefaultRequestHeaders.Add("ts", ts);
        client.DefaultRequestHeaders.Add("A-Signature", asignHex);

        var response = await client.PostAsync($"{host}/api/account/Login", formContent_login);
        var body = await response.Content.ReadAsStringAsync();
        callbackOnFinish(response.IsSuccessStatusCode);
    }

    public void OpenAuthorizeURL()
    {
        string uri = host + "/connect/Authorize?response_type=code&client_id=" + connectBasic.client_id + "&redirect_uri=" + redirect_uri + "&scope=game+offline_access&state=" + state;

        Application.OpenURL(uri);
    }


    public IEnumerator GetConnectToken_Coroutine(UnityAction<ConnectToken> callbackOnFinish)
    {
        if (code == "")
        {
            Debug.LogWarning("code error");
            callbackOnFinish(null);
        }

        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("client_id", connectBasic.client_id);
        form.AddField("client_secret", connectBasic.client_secret);
        form.AddField("redirect_uri", redirect_uri);
        form.AddField("grant_type", "authorization_code");

        string uri = host + "/connect/token";
        UnityWebRequest webRequest = UnityWebRequest.Post(uri, form);
        yield return webRequest.SendWebRequest();
        detectWWWRequestResult(webRequest, uri);
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            tokenData = ConnectToken.CreateFromJSON(webRequest.downloadHandler.text);
            access_token = tokenData.access_token;
            refresh_token = tokenData.refresh_token;
            callbackOnFinish(tokenData);
        }
        else
        {
            callbackOnFinish(null);
        }
    }

    public IEnumerator GetRefreshToken_Coroutine(UnityAction<ConnectToken> callbackOnFinish)
    {
        if (refresh_token == "")
        {
            Debug.LogWarning("refresh_token error");
            callbackOnFinish(null);
        }

        string uri = host + "/connect/token";

        WWWForm form = new WWWForm();
        form.AddField("refresh_token", refresh_token);
        form.AddField("client_id", connectBasic.client_id);
        form.AddField("client_secret", connectBasic.client_secret);
        form.AddField("redirect_uri", redirect_uri);
        form.AddField("grant_type", "refresh_token");

        UnityWebRequest webRequest = UnityWebRequest.Post(uri, form);
        yield return webRequest.SendWebRequest();
        detectWWWRequestResult(webRequest, uri);
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            tokenData = ConnectToken.CreateFromJSON(webRequest.downloadHandler.text);
            callbackOnFinish(tokenData);
        }
        else
        {
            callbackOnFinish(null);
        }
    }


    public IEnumerator GetMe_Coroutine(UnityAction<MeInfo> callbackOnFinish)
    {
        if (access_token == "")
        {
            Debug.LogWarning("access_token error");
            callbackOnFinish(null);
        }

        string timestamp = getTimestamp();
        string signdata = "?RequestNumber=" + requestNumber + "&Timestamp=" + timestamp;
        string X_Signature = getxSignature(signdata);

        string uri = game_api_host + "/api/Me" + signdata;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            string authorization = "Bearer " + access_token;
            webRequest.SetRequestHeader("Accept", "text/plain");
            webRequest.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
            webRequest.SetRequestHeader("Connection", "keep-alive");

            webRequest.SetRequestHeader("Authorization", authorization);
            webRequest.SetRequestHeader("X-Developer-Id", connectBasic.X_Developer_Id);
            webRequest.SetRequestHeader("X-Signature", X_Signature);
             
            yield return webRequest.SendWebRequest();
            detectWWWRequestResult(webRequest, uri);

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                me = MeInfo.CreateFromJSON(webRequest.downloadHandler.text);
                callbackOnFinish(me);
            }
            else
            {
                callbackOnFinish(null);
            }
        }
    }
     
    static string detectWWWRequestResult(UnityWebRequest www, string uri)
    {
        string Headers = "";
        foreach (var s in www.GetResponseHeaders())
        {
            Headers += s;
        }
        Debug.Log(Headers);

        string[] pages = uri.Split('/');
        int page = pages.Length - 1;
        switch (www.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(pages[page] + ": Error: " + www.error);
                return "ConnectionError +DataProcessingError " + www.error;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(pages[page] + ": HTTP Error: " + www.error);
                return "HTTP Error " + www.error;
            case UnityWebRequest.Result.Success:
                Debug.Log(pages[page] + ":\nReceived: " + www.downloadHandler.text);
                return "Received " + www.downloadHandler.text;
            default:
                return "";
        }
    }

    public static string getTimestamp()
    {

        DateTime dt = DateTime.Now; // Or whatever
        string s = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
        return s;
    }

    private string getxSignature(string data)
    {
        try
        {  
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(RSAxmlStr); 
            var bytes = Encoding.UTF8.GetBytes(data);

            // sign
            var signature = rsa.SignData(bytes, 0, bytes.Length, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var xSignature = Convert.ToBase64String(signature);

            // verify
            var isValid = rsa.VerifyData(bytes, signature, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            return xSignature;
        }
        catch (Exception e)
        {
            Debug.LogError("Exception RSA file: " + e);
            return null;
        }
    }

    #region INIT REQUEST
    // INIT REQUEST
    public void CreateAccountInitData(string _email, string _password)
    {
        this.accountRequestData = new AccountInitRequest
        {
            email = _email,
            password = _password
        };
    }
    #endregion
}
