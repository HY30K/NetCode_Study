using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}


public class UGSAuthWrapper
{
    public static AuthState State { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (State == AuthState.Authenticated)
        {
            return State;
        }
        if (State == AuthState.Authenticating) //이미 인증시도중이라면
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating(); //대기
            return State;
        }

        await SignInAnonymouslyAsync(maxTries);

        return State;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        State = AuthState.Authenticating; //인증시작

        int tries = 0;
        while (State == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                State = AuthState.Authenticated;
                break;
            }
            tries++;
            await Task.Delay(1000); //1초에 한번씩 재인증시도
        }
    }

    private static async Task<AuthState> Authenticating()
    {
        while (State == AuthState.Authenticating || State == AuthState.NotAuthenticated)
        {
            await Task.Delay(200); //0.2초 대기
        }

        return State;
    }
}
