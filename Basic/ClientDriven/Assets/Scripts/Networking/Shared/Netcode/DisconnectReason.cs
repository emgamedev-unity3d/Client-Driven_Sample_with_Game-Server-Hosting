using UnityEngine;

public static class DisconnectReason
{
    static public ConnectStatus Reason { get; private set; } = ConnectStatus.Undefined;

    static public bool HasTransitionReason => Reason != ConnectStatus.Undefined;

    static public void SetDisconnectReason(ConnectStatus reason)
    {
        Debug.Assert(reason != ConnectStatus.Success);

        Reason = reason;
    }

    static public void Clear()
    {
        Reason = ConnectStatus.Undefined;
    }
}