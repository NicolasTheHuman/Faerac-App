using System;
using System.Collections.Generic;

[Serializable]
public class Promocion
{
    public int id;
    public int comercio_id;
    public string comercio_nombre;
    public string titulo;
    public string descripcion;
    public string descuento;
    public string fecha_inicio;
    public string fecha_fin;
    public int activa;
    public string created_at;
}

[Serializable]
public class PromocionesResponse
{
    public List<Promocion> records;
}