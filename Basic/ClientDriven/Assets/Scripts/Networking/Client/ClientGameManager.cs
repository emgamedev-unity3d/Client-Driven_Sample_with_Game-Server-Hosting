using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientGameManager : IDisposable
{
    public bool IsMatchmaking => m_matchmaker != null &&
        m_matchmaker.IsMatchmaking;
    public bool IsCancelingMatchmaking => m_matchmaker != null &&
        m_matchmaker.IsCanceling;

    private MatchplayNetworkClient m_networkClient;

    private MatchmakingMatchmaker m_matchmaker;

    public UserData UserData { get; private set; }

    public bool Initialized { get; private set; } = false;
    
    public ClientGameManager()
    {
        string tempId = Guid.NewGuid().ToString();

        UserData = new UserData(
            "Player",
            tempId,
            0);
    }

    public async Task StartGameClientAsync()
    {
        m_networkClient = new MatchplayNetworkClient();
        m_matchmaker = new MatchmakingMatchmaker();

        var authenticationState = await AuthenticationWrapper.DoAuthentication();

        string authId;
        if (authenticationState == AuthenticationState.Authenticated)
        {
            authId = AuthenticationWrapper.PlayerID();
        }
        else
        {
            authId = Guid.NewGuid().ToString();
        }

        Debug.Log($"did Auth?{authenticationState} {authId}");
        Initialized = true;
    }

    public async Task CancelMatchmakingAsync()
    {
        await m_matchmaker.CancelMatchmakingAsync();
    }

    public void BeginConnection(string ip, int port)
    {
        Debug.Log($"Starting networkClient @ {ip}:{port}\nWith : {UserData}");

        m_networkClient.StartClient(ip, port, UserData);
    }

    public void Disconnect()
    {
        m_networkClient.DisconnectClient();
    }

    public async Task MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
    {
        if (m_matchmaker.IsMatchmaking)
        {
            Debug.LogWarning("Already matchmaking, please wait or cancel.");
            return;
        }

        MatchmakerPollingResult matchResult = await GetMatchAsync();
        onMatchmakeResponse?.Invoke(matchResult);
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        Debug.Log($"Beginning Matchmaking with {UserData}");
        MatchmakingResult matchmakingResult = 
            await m_matchmaker.MatchmakeAsync(UserData.userAuthId);

        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            BeginConnection(matchmakingResult.ip, matchmakingResult.port);
        }
        else
        {
            Debug.LogWarning($"{matchmakingResult.result} : {matchmakingResult.resultMessage}");
        }

        return matchmakingResult.result;
    }

    public async Task CancelMatchmaking()
    {
        await m_matchmaker.CancelMatchmakingAsync();
    }

    public void ExitGame()
    {
        Dispose();
        Application.Quit();
    }

    public void Dispose()
    {
        m_networkClient?.Dispose();
        m_matchmaker?.Dispose();
    }
}