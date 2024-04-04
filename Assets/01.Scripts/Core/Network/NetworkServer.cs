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
    //클라이언트 ID를 넣고 AuthID를 받는 딕셔너리
    private Dictionary<ulong, string> _clientIdToAuthDictionary = new Dictionary<ulong, string>();
    //AuthID로 유저데이터를 받는 딕셔너리
    private Dictionary<string, UserData> _authIdToUserDataDictionary = new Dictionary<string, UserData>();


    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager; //캐싱
        _networkManager.ConnectionApprovalCallback += HandleApprovalCheck; //승인요청시

        _networkManager.OnServerStarted += HandleServerStarted;
    }

    //클라이언트들이 서버에 접속할 때 실행시켜서 승인할지 말지를 결정하도록 한다. 이때 요청과 응답이 넘어온다.
    private void HandleApprovalCheck(
        NetworkManager.ConnectionApprovalRequest req,
        NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();

        _clientIdToAuthDictionary[req.ClientNetworkId] = data.userAuthId;
        _authIdToUserDataDictionary[data.userAuthId] = data;

        //        Debug.Log(data.username);

        res.Approved = true; //승인 처리 완료로 응답을 보낼 것
        res.CreatePlayerObject = true;
    }

    //서버가 시작되면 나오는 콜백
    private void HandleServerStarted()
    {
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;

    }

    private void HandleClientDisconnect(ulong clientID)
    {
        //클라이언트 접속해제시 딕셔너리에서도 삭제.
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
