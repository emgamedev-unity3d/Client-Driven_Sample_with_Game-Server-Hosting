using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public enum AuthenticationState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimedOut
}

public static class AuthenticationWrapper
{
    public static AuthenticationState AuthorizationState { get; private set; } 
        = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuthentication(int tries = 5)
    {
        //If we are already authenticated, just return Authenticated
        if (AuthorizationState == AuthenticationState.Authenticated)
        {
            return AuthorizationState;
        }

        if (AuthorizationState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Cant Authenticate if we are authenticating or authenticated");

            await Authenticating();

            return AuthorizationState;
        }

        await SignInAnonymouslyAsync(tries);
        Debug.Log($"Auth attempts Finished : {AuthorizationState}");

        return AuthorizationState;
    }

    //Awaitable task that will pass the clientID once authentication is done.
    public static string PlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    //Awaitable task that will pass once authentication is done.
    public static async Task<AuthenticationState> Authenticating()
    {
        while (AuthorizationState == AuthenticationState.Authenticating ||
            AuthorizationState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthorizationState;
    }

    static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthorizationState = AuthenticationState.Authenticating;
        var tries = 0;
        while (AuthorizationState == AuthenticationState.Authenticating && tries < maxRetries)
        {
            try
            {
                //To ensure staging login vs non staging
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && 
                    AuthenticationService.Instance.IsAuthorized)
                {
                    AuthorizationState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(ex);
                AuthorizationState = AuthenticationState.Error;
            }
            catch (RequestFailedException exception)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(exception);
                AuthorizationState = AuthenticationState.Error;
            }

            tries++;

            await Task.Delay(1000);
        }

        if (AuthorizationState != AuthenticationState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tries} attempts");
            AuthorizationState = AuthenticationState.TimedOut;
        }
    }

    public static void SignOut()
    {
        AuthenticationService.Instance.SignOut(false);
        AuthorizationState = AuthenticationState.NotAuthenticated;
    }
}