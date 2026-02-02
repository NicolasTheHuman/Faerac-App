using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TogglePassword : MonoBehaviour
{
    [SerializeField] private TMP_InputField _passwordField;

    [SerializeField] private Image _eyeImage;
    [SerializeField] private Sprite _closedEye;
    [SerializeField] private Sprite _openedEye;
    [SerializeField] private bool _showing;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _passwordField.contentType = TMP_InputField.ContentType.Password;
        _passwordField.ForceLabelUpdate();
        _showing = false;
    }

    public void TogglePasswordVisibility()
    {
        _showing = !_showing;
        _passwordField.contentType = _showing ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        _eyeImage.sprite = _showing ? _openedEye : _closedEye;
        _passwordField.ForceLabelUpdate();
    }
}
