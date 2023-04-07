using System.Threading.Tasks;
using Unity.Netcode;

public class ServerSingleton : SingletonPersistent<ServerSingleton>
{
    private ServerGameManager m_serverGameManager;

    public async Task StartGameServerAsync()
    {
        m_serverGameManager = new ServerGameManager(
            ApplicationStartupData.IP(),
            ApplicationStartupData.Port(),
            ApplicationStartupData.QPort(),
            NetworkManager.Singleton);

        await m_serverGameManager.StartGameServerAsync();
    }

    private void OnDestroy()
    {
        m_serverGameManager?.Dispose();
    }
}
