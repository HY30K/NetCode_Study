using DG.Tweening;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_InputField _inputName;
    [SerializeField] private Button _btnLogin;

    private void Awake()
    {
        _btnLogin.interactable = false;
        _btnLogin.onClick.AddListener(HandleLoginBtnClick);
        _inputName.onValueChanged.AddListener(ValidateUserName);
    }

    private void ValidateUserName(string name)
    {
        //골뱅이 기호 사용시 이스케이프 문자 무시
        Regex regex = new Regex(@"[a-zA-Z0-9]{2,12}");
        bool success = regex.IsMatch(name);

        _btnLogin.interactable = success;
    }


    private void HandleLoginBtnClick()
    {
        ClientSingleton.Instance.GameManager.SetPlayerName(_inputName.text);
        _canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        });
    }
}
