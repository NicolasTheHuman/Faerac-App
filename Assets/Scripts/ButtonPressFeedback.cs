using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float _pressedScale = 0.95f;
    [SerializeField] private float _pressedAlpha = 0.85f;
    [SerializeField] private float _duration = 0.1f;

    private RectTransform _rect;
    private CanvasGroup _group;
    private bool _pressed;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _group = GetComponent<CanvasGroup>();
        if (_group == null)
            _group = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        Animate(_pressedScale, _pressedAlpha);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Release();
    }

    private void Release()
    {
        if (!_pressed) return;
        _pressed = false;
        Animate(1f, 1f);
    }

    private void Animate(float scale, float alpha)
    {
        _rect.DOKill();
        _rect.DOScale(scale, _duration).SetEase(Ease.OutQuad);

        _group.DOKill();
        _group.DOFade(alpha, _duration).SetEase(Ease.OutQuad);
    }
}
