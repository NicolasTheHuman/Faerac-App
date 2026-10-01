using System;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Barra tipo "scroll banner" en el panel de inicio: los turnos agendados en
/// este dispositivo (registro local de <see cref="AppointmentStore"/>) pasan
/// de derecha a izquierda, empezando por el más próximo. Si no hay turnos,
/// muestra un texto de noticia.
/// </summary>
public class TurnosInfoBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private RectTransform Viewport;
    [SerializeField] private float ScrollSpeed = 120f;
    [SerializeField] private string Separador = "          •          ";
    [SerializeField] private string NoticiaSinTurnos = "¡Ya estamos disponibles en Google Play y App Store!";

    private void OnEnable()
    {
        AppointmentStore.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        AppointmentStore.Changed -= Refresh;
    }

    private void Update()
    {
        if (InfoText == null || Viewport == null) return;

        var textRect = InfoText.rectTransform;
        var pos = textRect.anchoredPosition;
        pos.x -= ScrollSpeed * Time.deltaTime;

        if (pos.x <= -textRect.sizeDelta.x)
            pos.x = Viewport.rect.width;

        textRect.anchoredPosition = pos;
    }

    private void Refresh()
    {
        if (InfoText == null) return;

        var proximos = AppointmentStore.GetAll()
            .Select(r => new { Record = r, Fecha = GetFechaHora(r) })
            .Where(x => x.Fecha >= DateTime.Now)
            .OrderBy(x => x.Fecha)
            .ToList();

        InfoText.text = proximos.Count == 0
            ? NoticiaSinTurnos
            : string.Join(Separador, proximos.Select(x => $"Turno con {x.Record.profesionalNombre} - {x.Fecha:dd/MM} {x.Record.hora}"));

        InfoText.ForceMeshUpdate();

        var textRect = InfoText.rectTransform;
        textRect.sizeDelta = new Vector2(InfoText.preferredWidth, textRect.sizeDelta.y);
        textRect.anchoredPosition = new Vector2(Viewport != null ? Viewport.rect.width : 0f, textRect.anchoredPosition.y);
    }

    private static DateTime GetFechaHora(AppointmentRecord record)
    {
        if (!DateTime.TryParse(record.fecha, out var fecha))
            return DateTime.MaxValue;

        if (TimeSpan.TryParse(record.hora, out var hora))
            fecha += hora;

        return fecha;
    }
}
