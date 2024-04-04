using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    //Ŭ���̾�Ʈ ID�� �ְ� AuthID�� �޴� ��ųʸ�
    private Dictionary<ulong, string> _clientIdToAuthDictionary = new Dictionary<ulong, string>();
    //AuthID�� ���������͸� �޴� ��ųʸ�
    private Dictionary<string, UserData> _authIdToUserDataDictionary = new Dictionary<string, UserData>();


    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager; //ĳ��
        _networkManager.ConnectionApprovalCallback += HandleApprovalCheck; //���ο�û��

        _networkManager.OnServerStarted += HandleServerStarted;
    }

    //Ŭ���̾�Ʈ���� ������ ������ �� ������Ѽ� �������� ������ �����ϵ��� �Ѵ�. �̶� ��û�� ������ �Ѿ�´�.
    private void HandleApprovalCheck(
        NetworkManager.ConnectionApprovalRequest req,
        NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();

        _clientIdToAuthDictionary[req.ClientNetworkId] = data.userAuthId;
        _authIdToUserDataDictionary[data.userAuthId] = data;

        //        Debug.Log(data.username);

        res.Approved = true; //���� ó�� �Ϸ�� ������ ���� ��
        res.CreatePlayerObject = true;
    }

    //������ ���۵Ǹ� ������ �ݹ�
    private void HandleServerStarted()
    {
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;

    }

    private void HandleClientDisconnect(ulong clientID)
    {
        //Ŭ���̾�Ʈ ���������� ��ųʸ������� ����.
        if (_clientIdToAuthDictionary.TryGetValue(clientID, out string authID))
        {
            _clientIdToAuthDictionary.Remove(clientID);
            _authIdToUserDataDictionary.Remove(authID);
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) return;
        _networkManager.ConnectionApprovalCallback -= HandleApprovalCheck;
        _networkManager.OnServerStarted -= HandleServerStarted;

        _networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
