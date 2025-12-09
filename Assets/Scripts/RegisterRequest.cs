using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class RegisterRequest
{
    public string nombres;
    public string apellido;
    public string dni;
    public string password;
}

[Serializable]
public class RegisterResponse
{
    public string message;
}
