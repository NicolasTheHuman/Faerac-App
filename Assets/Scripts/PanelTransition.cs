using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PanelTransition : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private PanelTransition _previousCanvas;
    [SerializeField] private float _transitionTime = 0.2f;

    [Header("Buttons pop-in (auto-detected each Show)")]
    [SerializeField] private float _popInStartScale = 0.92f;
    [SerializeField] private float _popInDuration = 0.3f;

    public UnityEvent OnTransition;

    public CanvasGroup Canvas => _canvasGroup;
    public PanelTransition Previous => _previousCanvas;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        EnsureButtonPressFeedback();
    }

    public void Show()
    {
        SetVisible(true);
        PlayButtonsPopIn();
    }

    public void Hide()
    {
        SetVisible(false);
    }

    public void Hide(bool transition)
    {
        SetVisible(false);
        if(transition)
            OnTransition?.Invoke();
    }

    private void SetVisible(bool show)
    {
        StartCoroutine(ChangePanelAlpha(_canvasGroup, show));
    }

    IEnumerator ChangePanelAlpha(CanvasGroup panel , bool show)
    {
        var time = 0f;

        panel.blocksRaycasts = show;

        while (time < _transitionTime)
        {
            time += Time.deltaTime;
            panel.alpha = show ? Mathf.Lerp(0, 1, time / _transitionTime) : Mathf.Lerp(1, 0, time / _transitionTime);
            yield return null;
        }

        panel.interactable = show;
    }

    private void PlayButtonsPopIn()
    {
        var buttons = GetComponentsInChildren<Button>(false);
        foreach (var button in buttons)
        {
            var target = (RectTransform)button.transform;

            target.DOKill();
            target.localScale = Vector3.one * _popInStartScale;
            target.DOScale(1f, _popInDuration).SetEase(Ease.OutCubic);

            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
                group = target.gameObject.AddComponent<CanvasGroup>();

            group.DOKill();
            group.alpha = 0f;
            group.DOFade(1f, _popInDuration);
        }
    }

    private void EnsureButtonPressFeedback()
    {
        var buttons = GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            if (button.GetComponent<ButtonPressFeedback>() == null)
                button.gameObject.AddComponent<ButtonPressFeedback>();
        }
    }

}
