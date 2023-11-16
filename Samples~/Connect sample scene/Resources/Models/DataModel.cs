using System;
using UnityEngine;


namespace DataModel
{

    [System.Serializable]
    public class ConnectToken 
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public string scope;
        public string refresh_token;

        public static ConnectToken CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ConnectToken>(jsonString);
        }
    }

     
    [System.Serializable]
    public class CreatePayment 
    {
        public PaymentData data;
        public int status;
        public string message;
        public string requestNumber;
        public static CreatePayment  CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<CreatePayment>(jsonString);
        }
    }

    [System.Serializable]
    public class PaymentData
    {
        public string transactionId;
        public int eCoin;
        public int rebate;
        public string orderStatus;
    }

    [System.Serializable]
    public class GetApiPaymentResponse
    {
        public PaymentResultView data;

        public StatusCode status;

        public string message; 
        public System.Guid requestNumber;
        public static GetApiPaymentResponse CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<GetApiPaymentResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class PaymentResultView
    {
        public string transactionId;
        public int eCoin;
        public int rebate;
        public OrderStatuses orderStatus;
    }


    [System.Serializable]
    public class MeInfo
    {
        public MeData data;
        public StatusCode status;
        public string message;
        public string requestNumber;
        public static MeInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MeInfo>(jsonString);
        }
    }


    [System.Serializable]
    public class ConnectBasic
    {
        public string client_id;
        public string X_Developer_Id;
        public string client_secret;
        public string Game_id;
        public string referralCode;
        public static ConnectBasic CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ConnectBasic>(jsonString);
        } 
    } 

    [System.Serializable]
    public class MeData
    {
        public string email;
        public string nickName;
        public string avatarUrl;
        public int eCoin;
        public int rebate;
    }
}