using System;
using System.Threading.Tasks;

public class ClientSingleton : Singleton<ClientSingleton>
{
    public bool IsMatchmaking => m_clientGameManager != null &&
        m_clientGameManager.IsMatchmaking;

    public bool IsCanceling => m_clientGameManager != null &&
        m_clientGameManager.IsCancelingMatchmaking;

    private ClientGameManager m_clientGameManager = null;

    public async Task StartGameClientAsync()
    {
        m_clientGameManager = new ClientGameManager();

        await m_clientGameManager.StartGameClientAsync();
    }

    public async Task StartMatchmakingAsync(
        Action<MatchmakerPollingResult> onMatchmakeResponse)
    {
        await m_clientGameManager.MatchmakeAsync(onMatchmakeResponse);
    }

    public async Task CancelMatchmakingAsync()
    {
        await m_clientGameManager.CancelMatchmakingAsync();
    }

    private void OnDestroy()
    {
        m_clientGameManager?.Dispose();
    }
}