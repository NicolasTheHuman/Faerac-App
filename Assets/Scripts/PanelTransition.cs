using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class PanelTransition : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private PanelTransition _previousCanvas;
    [SerializeField] private float _transitionTime = 0.2f;
    
    public UnityEvent OnTransition;
    
    public CanvasGroup Canvas => _canvasGroup;
    public PanelTransition Previous => _previousCanvas;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        SetVisible(true);
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
    
}
