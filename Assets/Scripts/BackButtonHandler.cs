using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference _backAction;
    
    [SerializeField] private PanelTransition _startPanel;

    private PanelTransition _currentPanel;

    private void Start()
    {
        _currentPanel = _startPanel;
        _currentPanel.Show();
    }
    
    public void GoToPanel(PanelTransition next)
    {
        if (_currentPanel == next)
            return;

        next.Show();
        _currentPanel.Hide();
        _currentPanel = next;
    }
    
    public void GoToPanel(PanelTransition next, bool transition)
    {
        if (_currentPanel == next)
            return;

        next.Show();
        _currentPanel.Hide(transition);
        _currentPanel = next;
    }
    
    private void OnBackPressed(InputAction.CallbackContext ctx)
    {
        HandleBackAction();
    }

    private void HandleBackAction()
    {
        if (_currentPanel == null)
            return;

        if (_currentPanel.Previous == null)
        {
            ShowExitPopup();
            return;
        }


        GoToPanel(_currentPanel.Previous, true);
    }
    
    private void ShowExitPopup()
    {
        PopUpManager.Instance.ChangePopUpText("¿Seguro que quieres salir?");
        PopUpManager.Instance.ShowPopUp(Application.Quit);
    }
    
    private void OnEnable()
    {
        _backAction.action.Enable();
        _backAction.action.performed += OnBackPressed;
    }

    private void OnDisable()
    {
        _backAction.action.performed -= OnBackPressed;
        _backAction.action.Disable();
    }
}


