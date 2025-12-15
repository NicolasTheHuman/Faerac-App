using System;
using System.Collections.Generic;

[Serializable]
public class Comercio
{
    public int id;
    public string nombre;
    public string descripcion;
    public string categoria;
    public string direccion;
    public string telefono;
    public string horarios;
    public string latitud;
    public string longitud;
    public string created_at;
}


[Serializable]
public class ComerciosResponse
{
    public List<Comercio> records;
}