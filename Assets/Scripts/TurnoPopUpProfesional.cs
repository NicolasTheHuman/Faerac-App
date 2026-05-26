using UnityEngine;
using TMPro;

public class TurnoPopUpProfesional : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NombreText;
    [SerializeField] private TextMeshProUGUI CategoriaText;
    [SerializeField] private TextMeshProUGUI DescripcionText;
    [SerializeField] private GameObject ContinuarButton;

    public void Configurar(Profesional profesional)
    {
        NombreText.text = profesional.nombre;
        CategoriaText.text = profesional.categoria;
        DescripcionText.text = profesional.descripcion;
        ContinuarButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
            Debug.Log("Ir a Turno Calendar con: " + profesional.nombre);
        });
    }
}
