 using UnityEngine;

public class ToastHelper : MonoBehaviour
{
    AndroidJavaClass ajc;
    void Start()
    {
        ajc = new AndroidJavaClass("com.r17dame.connectplugin.UnityToast");
        SendToAndroid("_");
    }
    private void SendToAndroid(string message)
    {
        ajc.CallStatic("ShowToast", message);
    } 

    private void ReceiveFromAndroid(string message)
    {
        Debug.Log("Received message from toast plugin: " + message);
    }
}
