using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ApplicationStartup : MonoBehaviour
{
    [SerializeField]
    private GameObject m_clientManager;

    [SerializeField]
    private GameObject m_serverManager;

    private async void Start()
    {
        ApplicationStartupData.InitializeApplicationData();

        bool isServerBuild =
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        await UnityServices.InitializeAsync();

        if (isServerBuild)
        {
            m_clientManager.SetActive(false);

            m_clientManager.TryGetComponent(out ServerSingleton serverSingleton);

            await serverSingleton.StartGameServerAsync();

            return;
        }

        m_serverManager.SetActive(false);

        Debug.Log(
            @$"Starting client build, Graphics device type is {SystemInfo.graphicsDeviceType}");

        // TODO: start additional client data
    }
}