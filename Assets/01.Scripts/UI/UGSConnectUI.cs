using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UGSConnectUI : MonoBehaviour
{
    [SerializeField] private Button _relayHostBtn;
    [SerializeField] private Button _enterLobbyBtn;

    public UnityEvent OpenLobbyEvent;

    private void Awake()
    {
        _relayHostBtn.onClick.AddListener(HandleRelayHostClick);
        _enterLobbyBtn.onClick.AddListener(HandleEnterLobbyClick);
    }

    private void HandleEnterLobbyClick()
    {
        OpenLobbyEvent?.Invoke();
    }

    private async void HandleRelayHostClick()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

}
