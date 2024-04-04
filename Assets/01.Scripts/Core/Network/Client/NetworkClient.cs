using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong clientID)
    {
        //서버가 나간게 아니고 자기가 나간것도 아니라면 걍 무시
        if (clientID != 0 && clientID != _networkManager.LocalClientId) return;

        Disconnect();
    }

    public void Disconnect()
    {
        //씬을 이동시켜주고 현재 접속되어 있다면 연결을 끊어준다.
        if (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
        {
            SceneManager.LoadScene(SceneNames.MenuScene);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }
}
