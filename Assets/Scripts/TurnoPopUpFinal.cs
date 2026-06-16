using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnoPopUpFinal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ProfesionalText;
    [SerializeField] private TextMeshProUGUI FechaText;
    [SerializeField] private TextMeshProUGUI HoraText;
    [SerializeField] private GameObject AtrasButton;

    public int ProfesionalId { get; private set; }
    public string Fecha { get; private set; }
    public string Hora { get; private set; }
    public int NroInt { get; private set; }

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SuscribirAtras();
    }

    public void Configurar(int profesionalId, string profesionalNombre, DateTime fecha, string hora, int nroInt)
    {
        ProfesionalId = profesionalId;
        Fecha = fecha.ToString("yyyy-MM-dd");
        Hora = hora;
        NroInt = nroInt;

        if (ProfesionalText != null) ProfesionalText.text = profesionalNombre;
        if (FechaText != null) FechaText.text = fecha.ToString("dd/MM/yyyy");
        if (HoraText != null) HoraText.text = hora;
    }

    private void SuscribirAtras()
    {
        if (AtrasButton == null) return;

        var button = AtrasButton.GetComponent<Button>();
        if (button == null) return;

        button.onClick.RemoveListener(VolverAtras);
        button.onClick.AddListener(VolverAtras);
    }

    private void VolverAtras()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
