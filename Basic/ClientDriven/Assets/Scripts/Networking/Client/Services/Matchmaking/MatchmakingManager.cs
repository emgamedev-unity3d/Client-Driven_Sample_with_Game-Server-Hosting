using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class MatchmakingManager
{
    public bool IsMatchmaking { get; private set; }

    private CancellationTokenSource m_cancelToken;
    private string m_lastUsedTicket;

    private const string k_queueName = "casual-queue";
    private const int k_ticketCooldown = 1000;

    public async Task<MatchmakingResult> MatchmakeAsync(string userAuthId)
    {
        m_cancelToken = new CancellationTokenSource();

        var players = new List<Player>
        {
            new Player(userAuthId)
        };

        var createTicketOptions = new CreateTicketOptions(k_queueName);

        try
        {
            var createResult = 
                await MatchmakerService.Instance.CreateTicketAsync(
                    players,
                    createTicketOptions);

            m_lastUsedTicket = createResult.Id;

            try
            {
                while (!m_cancelToken.IsCancellationRequested)
                {
                    var checkTicket =
                        await MatchmakerService.Instance.GetTicketAsync(m_lastUsedTicket);

                    if (checkTicket.Type == typeof(MultiplayAssignment))
                    {
                        var matchAssignment = (MultiplayAssignment)checkTicket.Value;

                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Found)
                        {
                            return ReturnMatchmakingResult(
                                MatchmakerPollingResult.Success,
                                string.Empty,
                                matchAssignment);
                        }

                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Timeout ||
                            matchAssignment.Status == MultiplayAssignment.StatusOptions.Failed)
                        {
                            return ReturnMatchmakingResult(
                                MatchmakerPollingResult.MatchAssignmentError,
                                $"Ticket: {m_lastUsedTicket} - {matchAssignment.Status} - {matchAssignment.Message}",
                                null);
                        }
                        Debug.Log($"Polled Ticket: {m_lastUsedTicket} Status: {matchAssignment.Status} ");
                    }

                    await Task.Delay(k_ticketCooldown);
                }
            }
            catch (MatchmakerServiceException matchmakerServiceException)
            {
                return ReturnMatchmakingResult(
                    MatchmakerPollingResult.TicketRetrievalError,
                    matchmakerServiceException.ToString(),
                    null);
            }
        }

        catch (MatchmakerServiceException matchmakerServiceException)
        {
            return ReturnMatchmakingResult(
                MatchmakerPollingResult.TicketCreationError,
                matchmakerServiceException.ToString(),
                null);
        }

        return ReturnMatchmakingResult(
            MatchmakerPollingResult.TicketRetrievalError,
            "Cancelled Matchmaking",
            null);
    }

    public async Task CancelMatchmakingAsync()
    {
        if (!IsMatchmaking)
            return;

        IsMatchmaking = false;

        if (m_cancelToken.Token.CanBeCanceled)
            m_cancelToken.Cancel();

        if (string.IsNullOrEmpty(m_lastUsedTicket))
            return;

        Debug.Log($"Cancelling {m_lastUsedTicket}");

        await MatchmakerService.Instance.DeleteTicketAsync(m_lastUsedTicket);
    }

    private MatchmakingResult ReturnMatchmakingResult(
        MatchmakerPollingResult resultErrorType,
        string message,
        MultiplayAssignment assignment)
    {
        if (assignment != null)
        {
            string parsedIp = assignment.Ip;
            int? parsedPort = assignment.Port;

            if (parsedPort == null)
            {
                return new MatchmakingResult
                {
                    result = MatchmakerPollingResult.MatchAssignmentError,
                    resultMessage = $"Port missing? - {assignment.Port}\n-{assignment.Message}"
                };
            }

            return new MatchmakingResult
            {
                result = MatchmakerPollingResult.Success,
                ip = parsedIp,
                port = (int)parsedPort,
                resultMessage = assignment.Message
            };
        }

        return new MatchmakingResult
        {
            result = resultErrorType,
            resultMessage = message
        };
    }
}