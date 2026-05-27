using System;
using System.Collections.Generic;

[Serializable]
public class Profesional
{
    public int id;
    public string nombre;
    public string categoria;
    public string descripcion;
    public string whatsapp;
}

[Serializable]
public class ProfesionalesResponse
{
    public bool success;
    public List<Profesional> profesionales;
}
