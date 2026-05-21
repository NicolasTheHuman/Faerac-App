using UnityEngine;

public class TarjetaProfesional : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI NombreText;
    [SerializeField] private TMPro.TextMeshProUGUI CategoriaText;

    public void Configurar(Profesional profesional)
    {
        NombreText.text = profesional.nombre;
        CategoriaText.text = profesional.categoria;
    }
}
