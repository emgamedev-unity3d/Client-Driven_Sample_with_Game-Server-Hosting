using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class ServerGameManager : IDisposable
{
    public MatchplayNetworkServer NetworkServer { get; private set; }

    private readonly MultiplayAllocationService multiplayAllocationService;

    private string serverIP = "0.0.0.0";
    private int serverPort = 7777;
    private int queryPort = 7787;
    private string serverName = "Matchplay Server";
    private bool m_servicesAreStarted;

    private int playerCount;

    private const int k_multiplayServiceTimeoutMS = 20000;

    public ServerGameManager(
        string serverIP,
        int serverPort,
        int serverQPort,
        NetworkManager manager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = serverQPort;
        NetworkServer = new MatchplayNetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
        serverName = $"Server: {Guid.NewGuid()}";
    }

    public async Task StartGameServerAsync()
    {
        Debug.Log($"Starting server!");

        await multiplayAllocationService.BeginServerCheck();

        try
        {
            // Note: ATM the sample is not using payload information, so no problem in passing
            //  empty struct
            MatchmakingResults matchmakerPayload = new MatchmakingResults();
                //TODO: debug and find out what's causing possible race condition which
                //  can lead to a server-side crash
                // = await GetMatchmakerPayload(k_multiplayServiceTimeoutMS);

            if (matchmakerPayload != null)
            {
                Debug.Log($"Got payload: {matchmakerPayload}");

                SetAllocationData();

                NetworkServer.OnPlayerJoined += UserJoinedServer;
                NetworkServer.OnPlayerLeft += UserLeft;

                m_servicesAreStarted = true;
            }
            else
            {
                Debug.LogWarning("Getting the Matchmaker Payload timed out, starting with defaults.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Something went wrong trying to set up the Services:\n{ex} ");
        }

        if (!NetworkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("NetworkServer did not start as expected.");
            return;
        }
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload(int timeout)
    {
        if (multiplayAllocationService == null)
        { 
            return null;
        }

        var matchmakerPayloadTask =
            multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        //return the result of the payload task if done before task timeout delay
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout))
                == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        // if we reached here, the payload task timed out
        return null;
    }

    private void SetAllocationData()
    {
        multiplayAllocationService.SetServerName(serverName);
        multiplayAllocationService.SetMaxPlayers(4);
        multiplayAllocationService.SetBuildID("0");
    }

    private void UserJoinedServer(UserData userThatJoined)
    {
        Debug.Log($"{userThatJoined} joined the game");

        multiplayAllocationService.AddPlayer();
        playerCount++;
    }

    private void UserLeft(UserData userThatLeft)
    {
        Debug.Log($"{userThatLeft} left the game!");

        multiplayAllocationService.RemovePlayer();
        playerCount--;

        if (playerCount > 0)
            return;

        CloseServer();
    }

    private void CloseServer()
    {
        Debug.Log("Closing Server");
        Dispose();
        Application.Quit();
    }

    public void Dispose()
    {
        if (m_servicesAreStarted)
        {
            if (NetworkServer.OnPlayerJoined != null)
                NetworkServer.OnPlayerJoined -= UserJoinedServer;

            if (NetworkServer.OnPlayerLeft != null)
                NetworkServer.OnPlayerLeft -= UserLeft;
        }

        multiplayAllocationService?.Dispose();
        NetworkServer?.Dispose();
    }
}