using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnoPopUpFinal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ProfesionalText;
    [SerializeField] private TextMeshProUGUI FechaText;
    [SerializeField] private TextMeshProUGUI HoraText;
    [SerializeField] private TMP_InputField DniInputField;
    [SerializeField] private TMP_InputField CelularInputField;
    [SerializeField] private TMP_InputField EmailInputField;
    [SerializeField] private TMP_Dropdown DropdownObrasSociales;
    [SerializeField] private GameObject AtrasButton;
    [SerializeField] private GameObject ContinuarButton;

    public Profesional Profesional { get; private set; }
    public List<Mutual> ObrasSociales => Profesional?.obrasSociales;
    public int ProfesionalId { get; private set; }
    public string Fecha { get; private set; }
    public string Hora { get; private set; }
    public int NroInt { get; private set; }

    private CanvasGroup _canvasGroup;
    private int _codMutualSeleccionado;
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SuscribirAtras();
        SuscribirContinuar();
        ConfigurarInputFields();
    }

    private void ConfigurarInputFields()
    {
        if (DniInputField != null)
        {
            DniInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            DniInputField.onValueChanged.AddListener(value => SanitizarNumerico(DniInputField, value));
        }
        if (CelularInputField != null)
        {
            CelularInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            CelularInputField.onValueChanged.AddListener(value => SanitizarNumerico(CelularInputField, value));
        }
        if (EmailInputField != null)
        {
            EmailInputField.contentType = TMP_InputField.ContentType.EmailAddress;
        }
    }

    private void SanitizarNumerico(TMP_InputField field, string value)
    {
        var clean = Regex.Replace(value, @"[^\d]", "");
        if (clean != value)
            field.text = clean;
    }

    public void Configurar(Profesional profesional, DateTime fecha, string hora, int nroInt)
    {
        Profesional = profesional;
        ProfesionalId = profesional.id;
        Fecha = fecha.ToString("yyyy-MM-dd");
        Hora = hora;
        NroInt = nroInt;

        if (ProfesionalText != null) ProfesionalText.text = profesional.nombre;
        if (FechaText != null) FechaText.text = fecha.ToString("dd/MM/yyyy");
        if (HoraText != null) HoraText.text = hora;

        if (DropdownObrasSociales != null)
        {
            var options = new List<string>();
            DropdownObrasSociales.ClearOptions();
            options.Add("Seleccionar");
            if (ObrasSociales != null && ObrasSociales.Count > 0)
            {
                foreach (var obra in ObrasSociales)
                {
                    options.Add(obra.nombre);
                }
                DropdownObrasSociales.AddOptions(options);
            }
            DropdownObrasSociales.onValueChanged.RemoveListener(OnObraSocialCambiada);
            DropdownObrasSociales.onValueChanged.AddListener(OnObraSocialCambiada);
            DropdownObrasSociales.value = 0;
            _codMutualSeleccionado = 0;
        }
    }

    private void OnObraSocialCambiada(int index)
    {
        if (index <= 0 || ObrasSociales == null || index - 1 >= ObrasSociales.Count)
        {
            _codMutualSeleccionado = 0;
            return;
        }
        _codMutualSeleccionado = ObrasSociales[index - 1].codMutual;
    }

    private void SuscribirAtras()
    {
        if (AtrasButton == null) return;
        var button = AtrasButton.GetComponent<Button>();
        if (button == null) return;
        button.onClick.RemoveListener(VolverAtras);
        button.onClick.AddListener(VolverAtras);
    }

    private void SuscribirContinuar()
    {
        if (ContinuarButton == null) return;
        var button = ContinuarButton.GetComponent<Button>();
        if (button == null) return;
        button.onClick.RemoveListener(ConfirmarTurno);
        button.onClick.AddListener(ConfirmarTurno);
    }

    private bool ValidarCampos(out string error)
    {
        error = string.Empty;

        string dni = DniInputField != null ? DniInputField.text : "";
        string celular = CelularInputField != null ? CelularInputField.text : "";
        string email = EmailInputField != null ? EmailInputField.text : "";

        if (dni.Length < 7 || dni.Length > 8)
        {
            error = "DNI inválido (debe tener 7 u 8 dígitos)";
            return false;
        }
        if (celular.Length != 10)
        {
            error = "Celular inválido (debe tener 10 dígitos)";
            return false;
        }
        if (!EmailRegex.IsMatch(email))
        {
            error = "Email inválido";
            return false;
        }
        return true;
    }

    private async void ConfirmarTurno()
    {
        if (!ValidarCampos(out string error))
        {
            PopUpManager.Instance.ShowPopUp();
            PopUpManager.Instance.ChangePopUpText(error);
            return;
        }

        var request = new ConfirmarTurnoRequest
        {
            nro_int = NroInt,
            cod_profesional = ProfesionalId,
            fecha = Fecha,
            dni = DniInputField.text,
            celular = CelularInputField.text,
            email = EmailInputField.text,
            codMutual = _codMutualSeleccionado
        };

        Debug.Log($"Confirmando turno: {JsonUtility.ToJson(request)}");

        PopUpManager.Instance.ShowPopUp();

        var result = await APIClient.Instance.Post<ConfirmarTurnoResponse>("turnos/confirmar", request);

        if (result.Success && result.Data != null && result.Data.success)
        {
            PopUpManager.Instance.ChangePopUpText(result.Data.message);
        }
        else
        {
            string msg = result.Data != null ? result.Data.message : result.ErrorMessage;
            if (string.IsNullOrEmpty(msg)) msg = "Error al confirmar el turno";
            PopUpManager.Instance.ChangePopUpText(msg);
        }
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
