using Unity.Netcode;
using UnityEngine;

public class ClientSampleServerStartupCheck : MonoBehaviour
{
    void Start()
    {
        bool isServerBuild =
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        if (!isServerBuild)
        {
            Debug.Log(@$"Starting client build, Graphics device type is {
                SystemInfo.graphicsDeviceType
                }");

            return;
        }

        // TODO: react to server successful or not
        NetworkManager.Singleton.StartServer();
    }
}
