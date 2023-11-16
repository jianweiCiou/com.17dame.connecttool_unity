 

public enum OrderStatuses : byte
{
    Failed,
    WaitingForPayment,
    Processing,
    Completed,
    Refund,
    Cancel,
    PaymentExpired
}
public enum StatusCode
{
    Error = 999,
    TimestampNotAvailable,
    DeveloperNotExists,
    SignatureInvalid,
    UserNotExists,
    Success,
    GameOrderNoIsDuplicate,
    GameNotExists,
    ECoinNotEnough,
    GameOrderIsNotExists
}