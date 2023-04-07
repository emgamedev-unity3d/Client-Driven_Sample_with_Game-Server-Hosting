using System;

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthId;
    public ulong clientId;

    public int characterId = -1;

    public UserData(string userName, string userAuthId, ulong clientId)
    {
        this.userName = userName;
        this.userAuthId = userAuthId;
        this.clientId = clientId;
    }
}