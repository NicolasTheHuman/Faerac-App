using System;
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
        _popUpText.text = message;
    }
    
    public void ShowPopUp(UnityAction yesAction)
    {
        _yesBtn.onClick.RemoveAllListeners();
        _noBtn.onClick.RemoveAllListeners();
        
        _yesBtn.onClick.AddListener(yesAction);
        _noBtn.onClick.AddListener(HidePopUp);
        
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
        
        VisualizeCanvas(_popUpPanel, true);
        VisualizeCanvas(_yesNoPanel, true);
        VisualizeCanvas(_continuePanel,false);
    }
    
    public void ShowPopUp()
    {
        VisualizeCanvas(_popUpPanel,true);
        VisualizeCanvas(_continuePanel, true);
        VisualizeCanvas(_yesNoPanel, false);
    }

    public void HidePopUp()
    {
        VisualizeCanvas(_popUpPanel, false);
    }

    private void VisualizeCanvas(CanvasGroup canvas, bool show)
    {
        canvas.alpha = show ? 1 : 0;
        canvas.interactable = show;
        canvas.blocksRaycasts = show;
    }

}
