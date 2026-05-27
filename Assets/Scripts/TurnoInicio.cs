using System.Collections.Generic;
using UnityEngine;

public class TurnoInicio : MonoBehaviour
{
    private List<Profesional> _profesionales;
    private List<Profesional> _profesionalesFiltrados;
    [SerializeField] private GameObject PrefabTarjetaProfesional;
    [SerializeField] private GameObject GridProfesionales;
    [SerializeField] private GameObject PopUpProfesional;

    public async void ObtenerProfesionales()
    {
        PopUpManager.Instance.ShowPopUp();

        var response = await APIClient.Instance.Get<ProfesionalesResponse>("turnos/profesionales",
            error => Debug.LogError("Error al obtener profesionales: " + error));

        if (response == null || !response.success || response.profesionales == null) return;

        _profesionales = response.profesionales;
        foreach (var profesional in _profesionales) {
            GameObject tarjeta = Instantiate(PrefabTarjetaProfesional, GridProfesionales.transform);
            tarjeta.name = profesional.nombre;
            tarjeta.GetComponent<TarjetaProfesional>().Configurar(profesional);
            tarjeta.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                PopUpProfesional.GetComponent<TurnoPopUpProfesional>().Configurar(profesional);
                PopUpProfesional.SetActive(true);
            });
        }

        PopUpManager.Instance.HidePopUp();
    }

    public void FiltrarNombre(string nombre)
    {
        if (_profesionales == null) return;

        _profesionalesFiltrados = _profesionales.FindAll(p => p.nombre.ToLower().Contains(nombre.ToLower()));

        foreach (Transform child in GridProfesionales.transform) {
            child.gameObject.SetActive(false);
        }

        foreach (var profesional in _profesionalesFiltrados) {
            Transform tarjeta = GridProfesionales.transform.Find(profesional.nombre);
            if (tarjeta != null) {
                tarjeta.gameObject.SetActive(true);
            }
        }
    }
}
