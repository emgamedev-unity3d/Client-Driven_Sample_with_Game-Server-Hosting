using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class MatchplayNetworkClient : IDisposable
{
    public event Action<ConnectStatus> OnLocalConnection;
    public event Action<ConnectStatus> OnLocalDisconnection;

    private NetworkManager m_networkManager;

    private const int TimeoutDuration = 10;

    public MatchplayNetworkClient()
    {
        m_networkManager = NetworkManager.Singleton;
        m_networkManager.OnClientDisconnectCallback += RemoteDisconnect;
    }

    public void StartClient(string ip, int port, UserData userData)
    {
        var unityTransport = m_networkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, (ushort)port, "0.0.0.0");

        ConnectClient(userData);
    }

    public void DisconnectClient()
    {
        NetworkShutdown();
    }

    private void ConnectClient(UserData userData)
    {
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        m_networkManager.NetworkConfig.ConnectionData = payloadBytes;
        m_networkManager.NetworkConfig.ClientConnectionBufferTimeout = TimeoutDuration;

        if (m_networkManager.StartClient())
        {
            Debug.Log("Starting Client!");

            RegisterListeners();
        }
        else
        {
            Debug.LogWarning("Could not start Client!");
            OnLocalDisconnection?.Invoke(ConnectStatus.Undefined);
        }
    }

    public void RegisterListeners()
    {
        MatchplayNetworkMessenger.RegisterListener(
            NetworkMessage.LocalClientConnected,
            ReceiveLocalClientConnectStatus);

        MatchplayNetworkMessenger.RegisterListener(
            NetworkMessage.LocalClientDisconnected,
            ReceiveLocalClientDisconnectStatus);
    }

    private void ReceiveLocalClientConnectStatus(ulong clientId, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ConnectStatus status);

        Debug.Log("ReceiveLocalClientConnectStatus: " + status);

        if (status != ConnectStatus.Success)
        {
            DisconnectReason.SetDisconnectReason(status);
        }

        OnLocalConnection?.Invoke(status);
    }

    private void ReceiveLocalClientDisconnectStatus(ulong clientId, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ConnectStatus status);

        Debug.Log("ReceiveLocalClientDisconnectStatus: " + status);
        
        DisconnectReason.SetDisconnectReason(status);
    }

    private void RemoteDisconnect(ulong clientId)
    {
        Debug.Log($"Got Client Disconnect callback for {clientId}");

        if (clientId != 0 && clientId != m_networkManager.LocalClientId)
            return;

        NetworkShutdown();
    }

    private void NetworkShutdown()
    {
        // If we are already on the main menu then it means we timed-out
        if (m_networkManager.IsConnectedClient)
        {
            m_networkManager.Shutdown();
        }

        OnLocalDisconnection?.Invoke(DisconnectReason.Reason);
    }

    public void Dispose()
    {
        if (m_networkManager != null && m_networkManager.CustomMessagingManager != null)
        {
            m_networkManager.OnClientConnectedCallback -= RemoteDisconnect;
        }
    }
}