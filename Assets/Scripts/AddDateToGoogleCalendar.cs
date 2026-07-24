using System;
using UnityEngine;

public class AddDateToGoogleCalendar : MonoBehaviour
{
    [SerializeField] private TurnoPopUpFinal turnoFin;
    [SerializeField] private int duracionMinutos = 30;

    public void AddDateToCalendar()
    {
        if (turnoFin == null)
        {
            Debug.LogError("TurnoPopUpFinal no asignado en AddDateToGoogleCalendar.");
            return;
        }

        if (!DateTime.TryParse(turnoFin.Fecha + " " + turnoFin.Hora, out DateTime inicio))
        {
            Debug.LogError($"No se pudo parsear la fecha/hora del turno: {turnoFin.Fecha} {turnoFin.Hora}");
            return;
        }

        DateTime fin = inicio.AddMinutes(duracionMinutos);

        string start = inicio.ToString("yyyyMMddTHHmmss");
        string end = fin.ToString("yyyyMMddTHHmmss");

        string texto = Uri.EscapeDataString(
            $"Turno - {turnoFin.Profesional?.nombre ?? "Profesional"}");

        string detalles = Uri.EscapeDataString(
            $"Turno con {turnoFin.Profesional?.nombre ?? "profesional"} el {turnoFin.Fecha} a las {turnoFin.Hora}.");

        string calendarUrl =
            "https://www.google.com/calendar/render?action=TEMPLATE" +
            "&text=" + texto +
            "&dates=" + start + "/" + end +
            "&details=" + detalles +
            "&ctz=Etc/GMT+3";

        Application.OpenURL(calendarUrl);
    }
}