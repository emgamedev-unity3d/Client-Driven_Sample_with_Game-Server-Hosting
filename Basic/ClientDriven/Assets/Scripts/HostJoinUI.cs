using System;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

public class HostJoinUI : MonoBehaviour
{
    [SerializeField]
    UIDocument m_MainMenuUIDocument;

    [SerializeField]
    UIDocument m_InGameUIDocument;

    VisualElement m_MainMenuRootVisualElement;
    
    VisualElement m_InGameRootVisualElement;
    
    Button m_HostButton;
    
    Button m_ServerButton;
    
    Button m_ClientButton;

    Button m_FindMatchButton;
    Label m_queueStatusLabel;

    TextField m_IPAddressTextField;
    
    TextField m_PortTextField;

    float m_timeInQueue = 0f;

    void Awake()
    {
        m_MainMenuRootVisualElement = m_MainMenuUIDocument.rootVisualElement;
        m_InGameRootVisualElement = m_InGameUIDocument.rootVisualElement;
        
        m_HostButton = m_MainMenuRootVisualElement.Query<Button>("HostButton");
        m_ClientButton = m_MainMenuRootVisualElement.Query<Button>("ClientButton");
        m_ServerButton = m_MainMenuRootVisualElement.Query<Button>("ServerButton");
        m_FindMatchButton = m_MainMenuRootVisualElement.Query<Button>("FindMatchButton");
        m_queueStatusLabel = m_MainMenuRootVisualElement.Query<Label>("queueStatusLabel");

        m_IPAddressTextField = m_MainMenuRootVisualElement.Query<TextField>("IPAddressField");
        m_PortTextField = m_MainMenuRootVisualElement.Query<TextField>("PortField");
        
        m_HostButton.clickable.clickedWithEventInfo += StartHost;
        m_ServerButton.clickable.clickedWithEventInfo += StartServer;
        m_ClientButton.clickable.clickedWithEventInfo += StartClient;
        m_FindMatchButton.clickable.clicked += FindMatch;
    }

    void Start()
    {
        ToggleMainMenuUI(true);
        ToggleInGameUI(false);
    }

    private void Update()
    {
        if (ClientSingleton.Instance == null)
            return;

        if (ClientSingleton.Instance.IsMatchmaking &&
            !ClientSingleton.Instance.IsCanceling)
        {
            m_timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(m_timeInQueue);
            m_queueStatusLabel.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
        else
        {
            m_queueStatusLabel.text = string.Empty;
        }
    }

    void OnDestroy()
    {
        m_HostButton.clickable.clickedWithEventInfo -= StartHost;
        m_ServerButton.clickable.clickedWithEventInfo -= StartServer;
        m_ClientButton.clickable.clickedWithEventInfo -= StartClient;
    }

    void StartHost(EventBase obj)
    {
        SetUtpConnectionData();
        var result = NetworkManager.Singleton.StartHost();
        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    void StartClient(EventBase obj)
    {
        SetUtpConnectionData();

        var result = NetworkManager.Singleton.StartClient();
        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    void StartServer(EventBase obj)
    {
        SetUtpConnectionData();
        var result = NetworkManager.Singleton.StartServer();
        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    async void FindMatch()
    {
        if (ClientSingleton.Instance.IsCanceling)
            return;

        if(ClientSingleton.Instance.IsMatchmaking)
        {
            m_FindMatchButton.text = "Cancelling";

            await ClientSingleton.Instance.CancelMatchmakingAsync();

            m_FindMatchButton.text = "Find Match";
            return;
        }

        _ = ClientSingleton.Instance.StartMatchmakingAsync(OnMatchMade);

        m_FindMatchButton.text = "Cancel";
        m_queueStatusLabel.text = "Searching...";
        m_timeInQueue = 0f;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                m_queueStatusLabel.text = "Connecting";
                ToggleInGameUI(true);
                ToggleMainMenuUI(false);
                break;

            case MatchmakerPollingResult.TicketCreationError:
                m_queueStatusLabel.text = "TicketCreationError";
                break;

            case MatchmakerPollingResult.TicketCancellationError:
                m_queueStatusLabel.text = "TicketCancellationError";
                break;

            case MatchmakerPollingResult.TicketRetrievalError:
                m_queueStatusLabel.text = "TicketRetrievalError";
                break;

            case MatchmakerPollingResult.MatchAssignmentError:
                m_queueStatusLabel.text = "MatchAssignmentError";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }

        ToggleInGameUI(true);
        ToggleMainMenuUI(false);
    }

    void ToggleMainMenuUI(bool isVisible)
    {
        m_MainMenuRootVisualElement.style.display = isVisible ?
            DisplayStyle.Flex : DisplayStyle.None;
    }
    
    void ToggleInGameUI(bool isVisible)
    {
        m_InGameRootVisualElement.style.display = isVisible ? 
            DisplayStyle.Flex : DisplayStyle.None;
    }

    void SetUtpConnectionData()
    {
        var sanitizedIPText = Sanitize(m_IPAddressTextField.text);
        var sanitizedPortText = Sanitize(m_PortTextField.text);

        ushort.TryParse(sanitizedPortText, out var port);
        
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(sanitizedIPText, port);
    }
    
    /// <summary>
    /// Sanitize user port InputField box allowing only alphanumerics and '.'
    /// </summary>
    /// <param name="dirtyString"> string to sanitize. </param>
    /// <returns> Sanitized text string. </returns>
    static string Sanitize(string dirtyString)
    {
        return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
    }
}
