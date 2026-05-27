using System;
using System.Collections.Generic;

[Serializable]
public class Mutual
{
    public int codMutual;
    public string nombre;
}

[Serializable]
public class ObrasSocialesResponse
{
    public bool success;
    public List<Mutual> mutuales;
}

[Serializable]
public class TurnosDisponiblesResponse
{
    public bool success;
    public List<string> turnos;
}

[Serializable]
public class Horario
{
    public int nroint;
    public string hora;
}

[Serializable]
public class HorariosResponse
{
    public bool success;
    public List<Horario> horarios;
}

[Serializable]
public class ConfirmarTurnoRequest
{
    public int nro_int;
    public int cod_profesional;
    public string fecha;
    public string dni;
    public string celular;
    public string email;
    public int codMutual;
}

[Serializable]
public class TurnoConfirmadoDatos
{
    public int nro_int;
    public int cod_profesional;
    public string fecha;
    public int cod_paciente;
    public bool es_nuevo;
    public string dni;
    public string celular;
    public string email;
    public int cod_mutual;
}

[Serializable]
public class ConfirmarTurnoResponse
{
    public bool success;
    public string message;
    public TurnoConfirmadoDatos datos;
}
