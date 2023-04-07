using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class ServerGameManager : IDisposable
{
    public MatchplayNetworkServer NetworkServer { get; private set; }

    private MultiplayAllocationService multiplayAllocationService;
    private string ConnectionString => $"{serverIP}:{serverPort}";
    private string serverIP = "0.0.0.0";
    private int serverPort = 7777;
    private int queryPort = 7787;
    private string serverName = "Matchplay Server";
    private bool startedServices;

    private int playerCount;

    private const int k_multiplayServiceTimeout = 20000;

    public ServerGameManager(
        string serverIP,
        int serverPort,
        int serverQPort,
        NetworkManager manager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = serverQPort;
        //NetworkServer = new MatchplayNetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
        serverName = $"Server: {Guid.NewGuid()}";
    }

    public async Task StartGameServerAsync()
    {
        Debug.Log($"Starting server!");

        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakerPayload = 
                await GetMatchmakerPayload(k_multiplayServiceTimeout);

            if (matchmakerPayload != null)
            {
                Debug.Log($"Got payload: {matchmakerPayload}");

                SetAllocationData();
                NetworkServer.OnPlayerJoined += UserJoinedServer;
                NetworkServer.OnPlayerLeft += UserLeft;
                startedServices = true;
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
        if (multiplayAllocationService == null) { return null; }

        var matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

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
    }
}