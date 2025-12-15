using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")] 
    [SerializeField] private CanvasGroup _welcomePanelCG;
    [SerializeField] private CanvasGroup _loginPanelCG;
    [SerializeField] private CanvasGroup _registerPanelCG;
    [SerializeField] private CanvasGroup _credentialPanelCG;
    [SerializeField] private CanvasGroup _discountsPanelCG;

    [SerializeField] private float _transitionTime = 0.2f;
    
    [Header("Register")]
    [SerializeField] private TMP_InputField _namesInput;
    [SerializeField] private TMP_InputField _lastnamesInput;
    [FormerlySerializedAs("_dniInput")] [SerializeField] private TMP_InputField _dniInputRegister;
    [FormerlySerializedAs("_passwordInput")] [SerializeField] private TMP_InputField _passwordInputRegister;
    
    [Header("Login")]
    [SerializeField] private TMP_InputField _dniInputLogin;
    [SerializeField] private TMP_InputField _passwordInputLogin;

    [Header("Credential")] 
    [SerializeField] private TMP_Text _credentialName;
    [SerializeField] private TMP_Text _credentialID;
    [SerializeField] private TMP_Text _credentialBirthdate;

    [Header("Shops")] 
    [SerializeField] private ContentSizeFitter _discountsContent;
    [SerializeField] private DiscountUIElement _discountsPrefab;
    
    
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

        if (response == null) 
            return;
        
        Debug.Log($"Register OK: {response.message} Usuario creado exitosamente");
        SaveUserDataToPlayerPrefs(new UserData()
        {
            nombres = body.nombres,
            apellido = body.apellido,
            dni = body.dni,
            password = body.password
        });
        LoadCredential();
            
        HidePanel(_registerPanelCG);
        ShowPanel(_credentialPanelCG);
    }

    [UsedImplicitly]
    public async void Login()
    {
        var body = new LoginRequest()
        {
            dni = _dniInputLogin.text,
            password = _passwordInputLogin.text,
        };

        LoginResponse response = await APIClient.Instance.Post<LoginResponse>("usuarios/login", body,
            err => Debug.LogError("Error Login: " + err)
        );

        if (response == null)
        {
            Debug.LogError("Login fail: null response");
            return;
        }
        
        
        Debug.Log("Login OK");
        Debug.Log($"Response {response.usuario}");
        Debug.Log("Usuario: " + response.usuario.nombres + " " + response.usuario.apellido);
        Debug.Log("DNI: " + response.usuario.dni);

        SaveUserDataToPlayerPrefs(response.usuario);
        LoadCredential();
        HidePanel(_loginPanelCG);
        ShowPanel(_credentialPanelCG);
    }

    public async void LoadShop(int id)
    {
        var response = await APIClient.Instance.Get<Comercio>($"comercios/{id}");

        if (response == null)
        {
            Debug.LogError("No shops received");
            return;
        }

        Debug.Log($"{response.nombre} / {response.categoria}");
    }
    
    public async void LoadShops()
    {
        var response = await APIClient.Instance.Get<ComerciosResponse>("comercios");

        if (response == null || response.records == null)
        {
            Debug.LogError("No shops received");
            return;
        }

        foreach (var shop in response.records)
        {
            Debug.Log($"{shop.nombre} / {shop.categoria}");
        }
    }
    
    public async void LoadShops(string category)
    {
        var response = await APIClient.Instance.Get<ComerciosResponse>($"comercios/categoria/{category}");

        if (response == null)
        {
            Debug.LogError("No shops received");
            return;
        }

        foreach (var shop in response.records)
        {
            Debug.Log($"{shop.nombre} / {shop.categoria}");
            Instantiate(_discountsPrefab, _discountsContent.transform).Initialize(shop);
        }
    }

    public async void LoadPromotions()
    {
        var response = await APIClient.Instance.Get<PromocionesResponse>("promociones");

        if (response?.records == null)
        {
            Debug.LogError("No Promotions");
            return;
        }

        foreach (var discount in response.records)
        {
            Debug.Log($"{discount.titulo} ({discount.comercio_nombre})");
        }
    }
    
    public async void LoadPromotions(int shopID)
    {
        var response = await APIClient.Instance.Get<PromocionesResponse>($"promociones/comercio/{shopID}");

        if (response?.records == null)
        {
            Debug.LogError("No Promotions");
            return;
        }

        foreach (var discount in response.records)
        {
            Debug.Log($"{discount.titulo} ({discount.comercio_nombre})");
        }
    }


    void SaveUserDataToPlayerPrefs(UserData userData)
    {
        PlayerPrefs.SetString("name", userData.nombres);
        PlayerPrefs.SetString("lastname", userData.apellido);
        PlayerPrefs.SetString("dni", userData.dni);
        PlayerPrefs.Save();
    }

    void LoadCredential()
    {
        Debug.Log("Credential");
        _credentialName.text = PlayerPrefs.GetString("name") + " " + PlayerPrefs.GetString("lastname");
        _credentialID.text = PlayerPrefs.GetString("dni");
    }
    
    public void HidePanel(CanvasGroup panelToHide)
    {
        StartCoroutine(ChangePanelAlpha(panelToHide, false));
    }

    public void ShowPanel(CanvasGroup panelToShow)
    {
        StartCoroutine(ChangePanelAlpha(panelToShow, true));
    }

    IEnumerator ChangePanelAlpha(CanvasGroup panel , bool show)
    {
        var time = 0f;

        panel.blocksRaycasts = show;

        while (time < _transitionTime)
        {
            time += Time.deltaTime;
            panel.alpha = show ? Mathf.Lerp(0, 1, time / _transitionTime) : Mathf.Lerp(1, 0, time / _transitionTime);
            yield return null;
        }

        panel.interactable = show;
    }


}
