using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AppointmentRecord
{
    public int nroInt;
    public int codProfesional;
    public string profesionalNombre;
    public string fecha;
    public string hora;
}

[Serializable]
internal class AppointmentRecordList
{
    public List<AppointmentRecord> items = new List<AppointmentRecord>();
}

/// <summary>
/// Registro local (solo en este teléfono) de los turnos agendados desde la
/// app, para poder identificar/cancelar sus recordatorios por nro_int.
/// </summary>
public static class AppointmentStore
{
    private const string PrefsKey = "faerac_turnos_agendados";

    /// <summary>Se dispara cuando se agrega o quita un turno del registro local.</summary>
    public static event Action Changed;

    public static List<AppointmentRecord> GetAll()
    {
        var json = PlayerPrefs.GetString(PrefsKey, "");
        if (string.IsNullOrEmpty(json)) return new List<AppointmentRecord>();
        return JsonUtility.FromJson<AppointmentRecordList>(json)?.items ?? new List<AppointmentRecord>();
    }

    public static void Add(AppointmentRecord record)
    {
        var list = GetAll();
        list.RemoveAll(r => r.nroInt == record.nroInt);
        list.Add(record);
        Save(list);
        Changed?.Invoke();
    }

    public static void Remove(int nroInt)
    {
        var list = GetAll();
        if (list.RemoveAll(r => r.nroInt == nroInt) > 0)
        {
            Save(list);
            Changed?.Invoke();
        }
    }

    private static void Save(List<AppointmentRecord> list)
    {
        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(new AppointmentRecordList { items = list }));
        PlayerPrefs.Save();
    }
}
