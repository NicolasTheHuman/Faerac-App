using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    [SerializeField] private CanvasGroup _popUpPanel;
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

    public void ShowPopUp()
    {
        _popUpPanel.alpha = 1;
        _popUpPanel.interactable = true;
        _popUpPanel.blocksRaycasts = true;
    }

    public void HidePopUp()
    {
        _popUpPanel.alpha = 0;
        _popUpPanel.interactable = false;
        _popUpPanel.blocksRaycasts = false;
    }
    
    
}
