using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    [SerializeField] private CanvasGroup _popUpPanel;
    [SerializeField] private CanvasGroup _continuePanel;
    [SerializeField] private CanvasGroup _yesNoPanel;
    [SerializeField] private TextMeshProUGUI _popUpText;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _yesBtn;
    [SerializeField] private Button _noBtn;
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private GameObject _loading;
    [SerializeField] private float _fadeDuration = 0.2f;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void ChangePopUpText(string message)
    {
        _loading.SetActive(false);
        _messagePanel.SetActive(true);
        _popUpText.text = message;
    }
    
    public void ShowPopUp(UnityAction yesAction)
    {
        _yesBtn.onClick.RemoveAllListeners();
        _noBtn.onClick.RemoveAllListeners();
        
        _yesBtn.onClick.AddListener(yesAction);
        _noBtn.onClick.AddListener(HidePopUp);
        
        _popUpPanel.transform.SetAsLastSibling();
        VisualizeCanvas(_popUpPanel, true);
        VisualizeCanvas(_yesNoPanel, true);
        VisualizeCanvas(_continuePanel,false);
    }

    public void ShowPopUp(UnityAction yesAction, UnityAction noAction)
    {
        _yesBtn.onClick.RemoveAllListeners();
        _noBtn.onClick.RemoveAllListeners();
        
        _yesBtn.onClick.AddListener(yesAction);
        _noBtn.onClick.AddListener(noAction);
        
        _popUpPanel.transform.SetAsLastSibling();
        VisualizeCanvas(_popUpPanel, true);
        VisualizeCanvas(_yesNoPanel, true);
        VisualizeCanvas(_continuePanel,false);
    }
    
    public void ShowPopUp()
    {
        _popUpPanel.transform.SetAsLastSibling();
        VisualizeCanvas(_popUpPanel,true);
        VisualizeCanvas(_continuePanel, true);
        VisualizeCanvas(_yesNoPanel, false);
    }

    public void HidePopUp()
    {
        _messagePanel.SetActive(false);
        _loading.SetActive(true);
        VisualizeCanvas(_popUpPanel, false);
    }

    private void VisualizeCanvas(CanvasGroup canvas, bool show)
    {
        canvas.interactable = show;
        canvas.blocksRaycasts = show;

        canvas.DOKill();
        canvas.DOFade(show ? 1f : 0f, _fadeDuration);
    }

}
