using System;
using UnityEngine;

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
    public UserData user;
}