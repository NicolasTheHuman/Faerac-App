using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Register")]
    [SerializeField] private TMP_InputField _namesInput;
    [SerializeField] private TMP_InputField _lastnamesInput;
    [FormerlySerializedAs("_dniInput")] [SerializeField] private TMP_InputField _dniInputRegister;
    [FormerlySerializedAs("_passwordInput")] [SerializeField] private TMP_InputField _passwordInputRegister;
    
    [Header("Login")]
    [SerializeField] private TMP_InputField _dniInputLogin;
    [SerializeField] private TMP_InputField _passwordInputLogin;
    
    [UsedImplicitly]
    public async void Register()
    {
        var body = new RegisterRequest
        {
            nombres = _namesInput.text,
            apellido = _lastnamesInput.text,
            dni = _dniInputRegister.text,
            password = _passwordInputRegister.text
        };
        
        var json = JsonUtility.ToJson(body);
        Debug.Log("JSON enviado: " + json);

        var response = await APIClient.Instance.Post<RegisterResponse>("usuarios/registrar", body, 
            error => Debug.Log($"Error {error}: Datos incompletos. Se requieren: nombres, apellido, dni y password"));
        
        if(response != null)
            Debug.Log($"Register OK: {response.message} Usuario creado exitosamente");
    }

    [UsedImplicitly]
    public async void Login()
    {
        var dni = _dniInputLogin.text;
        var password = _passwordInputLogin.text;

        var response = await APIClient.Instance.Login(
            dni, 
            password,
            err => Debug.LogError("Error Login: " + err)
        );

        if (response == null)
        {
            Debug.LogError("Login fail: null response");
            return;
        }

        if (response.user != null)
        {
            Debug.Log("Login OK");
            Debug.Log("Usuario: " + response.user.names + " " + response.user.lastnames);
            Debug.Log("DNI: " + response.user.dni);
        }
    }
}
