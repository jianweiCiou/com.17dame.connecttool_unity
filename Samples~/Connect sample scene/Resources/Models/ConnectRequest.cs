using UnityEngine;

[System.Serializable]
public class AccountInitRequest
{
    public string email;
    public string password; 
}
 

[System.Serializable]
public class PaymentRequest
{
    public string requestNumber;
    public string timestamp;
    public string gameId;
    public string orderNo;
    public int eCoin;
    public int rebate;
    public static PaymentRequest CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PaymentRequest>(jsonString);
    }
}
 


 