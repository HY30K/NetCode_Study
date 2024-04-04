using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _enterBtn;

    private RectTransform _rectTrm;
    public RectTransform Rect => _rectTrm;

    private Lobby _lobby;
    private LobbyPanel _panel;

    public void SetRoomTemplate(Lobby lobby, LobbyPanel panel)
    {
        _titleText.text = lobby.Name;
        _countText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        _lobby = lobby;

        _enterBtn.onClick.AddListener(HandleEnterBtnClick);
    }

    private void Awake()
    {
        _rectTrm = GetComponent<RectTransform>();
    }

    private async void HandleEnterBtnClick()
    {
        try
        {
            _panel.DisableInteraction(true);
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(_lobby.Id);
            //호스트 게임매니저에서 만들었던 Data 옵션의 JoinCode를 가져옴
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientWithJoinCode(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _panel.DisableInteraction(false);
        }
    }

}
