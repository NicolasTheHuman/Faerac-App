using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class RegisterRequest
{
    public string dni;
    public string password;
}

[Serializable]
public class RegisterResponse
{
    public string message;
    public UserData usuario;
}
