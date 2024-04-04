using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    private const string GameScene = "Game";
    // ����Ƽ ������ ������ �� Ŭ���̾�Ʈ�� ���� ������ �Ҵ����ִ� ����
    private Allocation _allocation;
    private string _joinCode;
    private string _lobbyId;
    private const int _maxConnections = 8;

    private NetworkServer _networkServer;

    private void MakeNetworkServer()
    {
        _networkServer = new NetworkServer(NetworkManager.Singleton);
    }

    public bool StartHostLocalNetwork()
    {

        MakeNetworkServer();
        if (NetworkManager.Singleton.StartHost())
        {

            NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
            return true;
        }
        else
        {
            //����Ƽ ��Ʈ��ũ �Ŵ��� �˴ٿ�.
            NetworkManager.Singleton.Shutdown();
            return false;
        }
    }


    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);

            MakeNetworkServer();

            if (NetworkManager.Singleton.StartHost())
            {

                NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);

            MakeNetworkServer();

            if (NetworkManager.Singleton.StartHost())
            {

                NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(_allocation, "dtls"); //udp���� ������ ���� ����
        transport.SetRelayServerData(relayServerData);

        string playerName = ClientSingleton.Instance.GameManager.PlayerName;
        //���⼭ �κ������� �޾ƿ´�.
        try
        {
            //�κ� ����� ���� �ɼǵ��� �ִ´�.
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false; //�κ� �ɼ��� ���� �־���� �Ѵ�. ���� �̰� true�� �ϸ� �����ڵ�θ� ���� ����

            //�ش� �κ� �ɼǿ� Join�ڵ带 �־��ش�. (Ŀ���ҵ����͸� �̷������� �ִ´�)
            // Visbilty Member�� �ش� �κ��� ����� �����Ӱ� ���� �� �ִٴ� ��.
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value:_joinCode)
                }
            };
            //�κ� �̸��� �ɼ��� �־��ֵ��� �Ǿ� ����.
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", _maxConnections, lobbyOptions);


            //�κ�� ��������� Ȱ���� ������ �ı��ǵ��ϵǾ� �ִ�. ���� �����ð��������� ping�� ������ �Ѵ�.
            _lobbyId = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15)); //15�ʸ��� ��
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return;
        }

        MakeNetworkServer();

        UserData userData = new UserData()
        {
            username = playerName,
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string json = JsonUtility.ToJson(userData);
        byte[] payload = Encoding.UTF8.GetBytes(json);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
        }
    }

    private IEnumerator HeartBeatLobby(float waitTimeSec)
    {
        var timer = new WaitForSecondsRealtime(waitTimeSec);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId); //�κ�� �� ������
            yield return timer;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        //��Ʈ��Ʈ �ڷ�ƾ ���ش�.
        HostSingleton.Instance.StopAllCoroutines();

        if (!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId); //���ö� �����
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        _lobbyId = string.Empty;
        _networkServer?.Dispose();
    }
}
