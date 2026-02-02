using System;
using System.Linq;
using UnityEngine;

public static class ScheduleUtils
{
    public static string GetTodayKey()
    {
        return DateTime.Now.DayOfWeek switch
        {
            DayOfWeek.Monday => "lunes",
            DayOfWeek.Tuesday => "martes",
            DayOfWeek.Wednesday => "miercoles",
            DayOfWeek.Thursday => "jueves",
            DayOfWeek.Friday => "viernes",
            DayOfWeek.Saturday => "sabado",
            DayOfWeek.Sunday => "domingo",
            _ => ""
        };
    }
    
    public static bool IsOpenNow(Comercio comercio)
    {
        if (comercio.horarios == null)
            return false;

        var today = ScheduleUtils.GetTodayKey();

        if (!comercio.horarios.TryGetValue(today, out var day))
            return false;

        if (day.cerrado || day.horarios == null)
            return false;

        var now = DateTime.Now.TimeOfDay;

        foreach (var range in day.horarios)
        {
            if (!TimeSpan.TryParse(range.apertura, out var open)) continue;
            if (!TimeSpan.TryParse(range.cierre, out var close)) continue;

            if (now >= open && now <= close)
                return true;
        }

        return false;
    }
    
    public static string GetTodayScheduleText(Comercio comercio)
    {
        if (comercio.horarios == null)
            return "Horarios no disponibles";

        var today = ScheduleUtils.GetTodayKey();

        if (!comercio.horarios.TryGetValue(today, out var day))
            return "Cerrado";

        if (day.cerrado || day.horarios.Count == 0)
            return "Cerrado";
        
        return string.Join(" / ",
            day.horarios.Select(h => $"{h.apertura}–{h.cierre}")
        );
    }


}

