using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoaderUI : MonoBehaviour
{
    private static LoaderUI _instance;
    public static LoaderUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LoaderUI>();
            }
            if (_instance == null)
            {
                Debug.LogError($"There is no LoaderUI");
            }
            return _instance;
        }
    }

    [SerializeField] private Image _loadImage;
    [SerializeField] private CanvasGroup _canvasGroup;

    private void Start()
    {
        _loadImage.raycastTarget = false;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void Show(bool value)
    {
        float fadeValue = value ? 1f : 0f;
        _canvasGroup.DOFade(fadeValue, 0.4f);
        _canvasGroup.blocksRaycasts = value;
        _canvasGroup.interactable = value;
    }
}

