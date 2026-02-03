using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaklySchedule
{
    public Dictionary<string, DaySchedule> dias;
}

[Serializable]
public class DaySchedule
{
    public bool cerrado;
    public List<TimeRange> horarios;
}


[Serializable]
public class TimeRange
{
    public string apertura;
    public string cierre;
}
