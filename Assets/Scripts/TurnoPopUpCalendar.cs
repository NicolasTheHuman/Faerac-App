using UnityEngine;
using TMPro;

public class TurnoPopUpCalendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ProfesionalNameText;

    public void Configurar(Profesional profesional)
    {
        ProfesionalNameText.text = profesional.nombre;
    }
}
