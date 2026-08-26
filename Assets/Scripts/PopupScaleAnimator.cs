using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupScaleAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform _card;
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _startScale = 0.92f;

    private Image _background;
    private CanvasGroup _cardGroup;
    private float _backgroundTargetAlpha;
    private Tween _scaleTween;
    private Tween _backgroundFadeTween;
    private Tween _cardFadeTween;

    private void Awake()
    {
        _background = GetComponent<Image>();
        if (_background != null)
            _backgroundTargetAlpha = _background.color.a;

        _cardGroup = _card.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _scaleTween?.Kill();
        _card.localScale = Vector3.one * _startScale;
        _scaleTween = _card.DOScale(1f, _duration).SetEase(Ease.OutCubic);

        if (_background != null)
        {
            _backgroundFadeTween?.Kill();
            _background.color = SetAlpha(_background.color, 0f);
            _backgroundFadeTween = _background.DOFade(_backgroundTargetAlpha, _duration);
        }

        if (_cardGroup != null)
        {
            _cardFadeTween?.Kill();
            _cardGroup.alpha = 0f;
            _cardFadeTween = _cardGroup.DOFade(1f, _duration);
        }
    }

    public void Close()
    {
        _scaleTween?.Kill();
        _scaleTween = _card.DOScale(_startScale, _duration).SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));

        if (_background != null)
        {
            _backgroundFadeTween?.Kill();
            _backgroundFadeTween = _background.DOFade(0f, _duration);
        }

        if (_cardGroup != null)
        {
            _cardFadeTween?.Kill();
            _cardFadeTween = _cardGroup.DOFade(0f, _duration);
        }
    }

    private static Color SetAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}
