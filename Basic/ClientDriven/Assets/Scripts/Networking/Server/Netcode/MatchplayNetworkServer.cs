using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class MatchplayNetworkServer : IDisposable
{
    public Action OnServerPlayerAdded;
    public Action OnServerPlayerRemoved;

    public Action<UserData> OnPlayerJoined;
    public Action<UserData> OnPlayerLeft;

    public Action<string> OnClientLeft;

    public static MatchplayNetworkServer Instance { get; private set; }

    public Dictionary<string, UserData> ClientData { get; private set; } = new();
    public Dictionary<ulong, string> ClientIdToAuth { get; private set; } = new();

    private NetworkManager m_networkManager;

    private const int k_maxConnectionPayload = 1024;
    private bool m_gameHasStarted;

    public MatchplayNetworkServer(NetworkManager networkManager)
    {
        Instance = this;

        m_networkManager = networkManager;

        m_networkManager.ConnectionApprovalCallback += ApprovalCheck;
        m_networkManager.OnServerStarted += OnNetworkReady;
    }

    public bool OpenConnection(string ip, int port)
    {
        var unityTransport = m_networkManager.gameObject.GetComponent<UnityTransport>();
        m_networkManager.NetworkConfig.NetworkTransport = unityTransport;
        unityTransport.SetConnectionData(ip, (ushort)port);

        Debug.Log($"Starting server at {ip}:{port}\n");

        return m_networkManager.StartServer();
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (request.Payload.Length > k_maxConnectionPayload || m_gameHasStarted)
        {
            response.Approved = false;
            response.CreatePlayerObject = false;
            response.Position = null;
            response.Rotation = null;
            response.Pending = false;

            return;
        }

        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        userData.clientId = request.ClientNetworkId;

        Debug.Log($"Host ApprovalCheck: connecting client: ({request.ClientNetworkId}) - {userData}");

        if (ClientData.ContainsKey(userData.userAuthId))
        {
            ulong oldClientId = ClientData[userData.userAuthId].clientId;
            Debug.Log($"Duplicate ID Found : {userData.userAuthId}, Disconnecting Old user");

            WaitToDisconnect(oldClientId);
        }

        ClientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        ClientData[userData.userAuthId] = userData;
        OnPlayerJoined?.Invoke(userData);

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Rotation = Quaternion.identity;
        response.Pending = false;
    }

    private async void WaitToDisconnect(ulong clientId)
    {
        await Task.Delay(500);

        m_networkManager.DisconnectClient(clientId);
    }

    private void OnNetworkReady()
    {
        m_networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientIdToAuth.TryGetValue(clientId, out string authId))
        {
            ClientIdToAuth?.Remove(clientId);
            OnPlayerLeft?.Invoke(ClientData[authId]);

            if (ClientData[authId].clientId == clientId)
            {
                ClientData.Remove(authId);
                OnClientLeft?.Invoke(authId);
            }
        }
    }

    public void Dispose()
    {
        if (m_networkManager == null)
            return;

        m_networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        m_networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        m_networkManager.OnServerStarted -= OnNetworkReady;

        if (m_networkManager.IsListening)
        {
            m_networkManager.Shutdown();
        }
    }
}
