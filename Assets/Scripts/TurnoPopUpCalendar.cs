using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.Dates;

public class TurnoPopUpCalendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ProfesionalNameText;
    [SerializeField] private DatePicker Calendar;

    private int _profesionalId;
    private int _lastNavMonth = -1;
    private int _lastNavYear = -1;

    public void Configurar(Profesional profesional)
    {
        ProfesionalNameText.text = profesional.nombre;
        _profesionalId = profesional.id;
        _lastNavMonth = -1;

        Calendar.SelectedDate = new SerializableDate();
        Calendar.VisibleDate = new SerializableDate(DateTime.Today);

        ConfigurarRangoFechas();
        BloquearAnioCompleto();
        SuscribirNavegacion();

        DateTime hoy = DateTime.Today;
        CargarMes(hoy.Month, hoy.Year);
        PrefetchMes(hoy.AddMonths(1).Month, hoy.AddMonths(1).Year);
    }

    private void ConfigurarRangoFechas()
    {
        DateTime hoy = DateTime.Today;

        Calendar.Config.DateRange.RestrictFromDate = true;
        Calendar.Config.DateRange.FromDate = new SerializableDate(hoy);

        Calendar.Config.DateRange.RestrictToDate = true;
        Calendar.Config.DateRange.ToDate = new SerializableDate(hoy.AddYears(1));

        Calendar.Config.DateRange.ProhibitedDates.Clear();
    }

    private void BloquearAnioCompleto()
    {
        var prohibidas = Calendar.Config.DateRange.ProhibitedDates;

        DateTime desde = Calendar.Config.DateRange.FromDate.Date;
        DateTime hasta = Calendar.Config.DateRange.ToDate.Date;

        for (DateTime f = desde; f <= hasta; f = f.AddDays(1))
            prohibidas.Add(new SerializableDate(f));

        Calendar.UpdateDisplay();
    }

    private void SuscribirNavegacion()
    {
        SubscribirNavegacionListeners();
        DatePickerTimer.DelayedCall(0.1f, SubscribirNavegacionListeners, this);
    }

    private void SubscribirNavegacionListeners()
    {
        if (Calendar == null) return;

        var navButtons = new[] {
            Calendar.Ref_Header.PreviousMonthButton,
            Calendar.Ref_Header.NextMonthButton,
            Calendar.Ref_Header.PreviousYearButton,
            Calendar.Ref_Header.NextYearButton
        };

        foreach (var btn in navButtons)
        {
            btn.Button.onClick.RemoveListener(AlNavegar);
            btn.Button.onClick.AddListener(AlNavegar);

            var hold = btn.Button.GetComponent<DatePicker_HoldButton>();
            if (hold != null)
            {
                hold.OnClickAction -= AlNavegar;
                hold.OnClickAction += AlNavegar;
            }
        }

        Calendar.Config.Events.OnDaySelected.RemoveListener(AlSeleccionarFecha);
        Calendar.Config.Events.OnDaySelected.AddListener(AlSeleccionarFecha);
    }

    private void AlNavegar()
    {
        DateTime visible = Calendar.VisibleDate.Date;
        int mes = visible.Month;
        int anio = visible.Year;

        bool retrocediendo = _lastNavMonth > 0 &&
            (anio < _lastNavYear || (anio == _lastNavYear && mes < _lastNavMonth));

        if (retrocediendo)
            CargarMes(mes, anio);

        DateTime next = visible.AddMonths(1);
        PrefetchMes(next.Month, next.Year);

        _lastNavMonth = mes;
        _lastNavYear = anio;
    }

    private void AlSeleccionarFecha(DateTime date)
    {
        DateTime next = Calendar.VisibleDate.Date.AddMonths(1);
        PrefetchMes(next.Month, next.Year);
    }

    private async void CargarMes(int mes, int anio)
    {
        PopUpManager.Instance.ShowPopUp();

        var response = await APIClient.Instance.Get<TurnosDisponiblesResponse>(
            $"turnos?profesional={_profesionalId}&mes={mes}&anio={anio}",
            error => Debug.LogError("Error al obtener turnos disponibles: " + error));

        PopUpManager.Instance.HidePopUp();

        if (response?.turnos is not { Count: > 0 }) return;

        ActualizarFechasProhibidas(mes, anio, response.turnos);
    }

    private async void PrefetchMes(int mes, int anio)
    {
        var response = await APIClient.Instance.Get<TurnosDisponiblesResponse>(
            $"turnos?profesional={_profesionalId}&mes={mes}&anio={anio}",
            error => { });

        if (response?.turnos is not { Count: > 0 }) return;

        ActualizarFechasProhibidas(mes, anio, response.turnos);
    }

    private void ActualizarFechasProhibidas(int mes, int anio, List<string> turnosDisponibles)
    {
        if (turnosDisponibles == null || turnosDisponibles.Count == 0) return;

        var prohibidas = Calendar.Config.DateRange.ProhibitedDates;
        prohibidas.RemoveAll(d => d.HasValue && d.Date.Month == mes && d.Date.Year == anio);

        var disponibles = new HashSet<string>(turnosDisponibles);
        int diasEnMes = DateTime.DaysInMonth(anio, mes);

        for (int dia = 1; dia <= diasEnMes; dia++)
        {
            var fecha = new DateTime(anio, mes, dia);
            if (!disponibles.Contains(fecha.ToString("yyyy-MM-dd")))
            {
                prohibidas.Add(new SerializableDate(fecha));
            }
        }

        Calendar.UpdateDisplay();
    }
}
