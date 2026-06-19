using UnityEngine;
using TMPro;
using Christina.UI;

public class TurnoPopUpProfesional : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NombreText;
    [SerializeField] private TextMeshProUGUI CategoriaText;
    [SerializeField] private TextMeshProUGUI DescripcionText;
    [SerializeField] private GameObject ObrasSocialesText;
    [SerializeField] private GameObject InformacionText;
    [SerializeField] private GameObject ObrasSocialesToggle;
    [SerializeField] private GameObject ContinuarButton;
    [SerializeField] private GameObject WhatsAppButton;
    [SerializeField] private GameObject PopUpCalendar;

    public void Configurar(Profesional profesional)
    {
        NombreText.text = profesional.nombre;
        CategoriaText.text = profesional.categoria;
        DescripcionText.text = profesional.descripcion;

        InformacionText.SetActive(true);
        ObrasSocialesText.SetActive(false);
        ObrasSocialesToggle.GetComponent<UnityEngine.UI.Slider>().value = 0;
        ObrasSocialesToggle.GetComponent<ToggleSwitch>().CurrentValue = false;

        if (string.IsNullOrEmpty(profesional.whatsapp)) {
            GetObrasSociales(profesional);
            WhatsAppButton.SetActive(false);
            ObrasSocialesToggle.SetActive(true);
            ContinuarButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            ContinuarButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            ContinuarButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                gameObject.SetActive(false);
                PopUpCalendar.GetComponent<TurnoPopUpCalendar>().Configurar(profesional);
                PopUpCalendar.SetActive(true);
            });
        } else {
            WhatsAppButton.SetActive(true);
            ObrasSocialesToggle.SetActive(false);
            ContinuarButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            WhatsAppButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            WhatsAppButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                Application.OpenURL("https://api.whatsapp.com/send?phone=" + profesional.whatsapp);
            });
        }
    }

    private async void GetObrasSociales(Profesional profesional)
    {
        PopUpManager.Instance.ShowPopUp();

        var response = await APIClient.Instance.Get<ObrasSocialesResponse>("turnos/mutuales?profesional=" + profesional.id,
            error => Debug.LogError("Error al obtener obras sociales: " + error));

        PopUpManager.Instance.HidePopUp();

        if (response == null || !response.success || response.mutuales == null) return;

        profesional.obrasSociales = response.mutuales;
        var nombres = response.mutuales.ConvertAll(m => m.nombre);
        ObrasSocialesText.GetComponentInChildren<TextMeshProUGUI>().text = string.Join("\n", nombres);
        ContinuarButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
}
