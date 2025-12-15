using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LoginRequest
{
    public string dni;
    public string password;
}

[Serializable]
public class LoginResponse
{
    public string message;
    public UserData usuario;
}