using DG.Tweening;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private LobbyUI _lobbyUIPrefab;
    [SerializeField] private float _spacing = 30f;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _refreshBtn;

    private List<LobbyUI> _lobbyList;

    private RectTransform _rectTrm;
    private CanvasGroup _canvasGroup;

    private bool _isRefreshing;

    private void Awake()
    {
        _lobbyList = new List<LobbyUI>();
        _rectTrm = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _closeBtn.onClick.AddListener(Close);
        _refreshBtn.onClick.AddListener(RefreshList);
    }

    private void Start()
    {
        float screenHeight = Screen.height;
        _rectTrm.anchoredPosition = new Vector2(0, screenHeight);
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
    }

    public void Open()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_rectTrm.DOAnchorPos(new Vector2(0, 0), 0.8f));
        seq.Join(_canvasGroup.DOFade(1f, 0.8f));
        seq.AppendCallback(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        });
        RefreshList();
    }


    public void Close()
    {
        float screenHeight = Screen.height;
        Sequence seq = DOTween.Sequence();
        seq.Append(_rectTrm.DOAnchorPos(new Vector2(0, screenHeight), 0.8f));
        seq.Join(_canvasGroup.DOFade(0f, 0.8f));
        seq.AppendCallback(() =>
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //CreateLobbyUI();
        }

    }

    public async void RefreshList()
    {
        if (_isRefreshing) return;
        _isRefreshing = true;
        DisableInteraction(true);

        try
        {
            //로비에 질의하기 위한 질의 객체
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25; //페이지네이션을 위한 한페이지에 몇개 옵션
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field:QueryFilter.FieldOptions.AvailableSlots ,
                    op: QueryFilter.OpOptions.GT,
                    value:"0"), //남아있는 칸이 0칸 초과인것들만
                new QueryFilter(
                    field:QueryFilter.FieldOptions.IsLocked ,
                    op: QueryFilter.OpOptions.EQ,
                    value:"0"),  //락이 0이면 락이되지 않은 애들만
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            //로비 비우고
            ClearLobbies();

            //다시 생성해주는 로직 여기에
            foreach (Lobby lobby in lobbies.Results)
            {
                CreateLobbyUI(lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }

        _isRefreshing = false;
        DisableInteraction(true);
    }

    //기존 있는 로비 지우기
    private void ClearLobbies()
    {
        foreach (LobbyUI ui in _lobbyList)
        {
            Destroy(ui.gameObject);
        }

        _lobbyList.Clear();
    }


    public void CreateLobbyUI(Lobby lobby)
    {
        LobbyUI ui = Instantiate(_lobbyUIPrefab, _scrollRect.content);

        ui.SetRoomTemplate(lobby, this);

        _lobbyList.Add(ui);
        float offset = _spacing;

        for (int i = 0; i < _lobbyList.Count; i++)
        {
            _lobbyList[i].Rect.anchoredPosition = new Vector2(0, -offset);
            offset += _lobbyList[i].Rect.sizeDelta.y + _spacing;
        }

        Vector2 contentSize = _scrollRect.content.sizeDelta;
        contentSize.y = offset;
        _scrollRect.content.sizeDelta = contentSize;
    }

    public void DisableInteraction(bool value)
    {
        _canvasGroup.interactable = !value;
        _canvasGroup.blocksRaycasts = !value;

        LoaderUI.Instance.Show(value);
    }
}

