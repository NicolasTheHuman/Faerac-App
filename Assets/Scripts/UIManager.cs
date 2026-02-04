using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")] 
    [SerializeField] private PanelTransition _welcomePanel;
    [SerializeField] private PanelTransition _loginPanel;
    [SerializeField] private PanelTransition _registerPanel;
    [SerializeField] private PanelTransition _credentialPanel;
    [SerializeField] private PanelTransition _locationPanel;
    [SerializeField] private PanelTransition _discountsPanel;

    [SerializeField] private float _transitionTime = 0.2f;
    [SerializeField] private BackButtonHandler _backButtonHandler;
    
    [Header("Register")]
    [SerializeField] private TMP_InputField _namesInput;
    [SerializeField] private TMP_InputField _lastnamesInput;
    [FormerlySerializedAs("_dniInput")] [SerializeField] private TMP_InputField _dniInputRegister;
    [FormerlySerializedAs("_passwordInput")] [SerializeField] private TMP_InputField _passwordInputRegister;
    
    [Header("Login")]
    [SerializeField] private TMP_InputField _dniInputLogin;
    [SerializeField] private TMP_InputField _passwordInputLogin;

    [Header("Credential")] 
    [SerializeField] private Image _profileImage;
    [SerializeField] private TMP_Text _credentialName;
    [SerializeField] private TMP_Text _credentialID;
    [SerializeField] private TMP_Text _credentialBirthdate;

    [Header("Shops")] 
    [SerializeField] private ContentSizeFitter _discountsContent;
    [SerializeField] private DiscountUIElement _discountsPrefab;

    [Header("Selected Shop")] 
    [SerializeField] private GameObject _selectedShopContainer;
    [SerializeField] private TMP_Text _selectedShopName;
    [SerializeField] private TMP_Text _selectedShopDescription;
    [SerializeField] private TMP_Text _selectedShopDiscount;
    
    [Space]
    public UnityEvent hidePanelEvent;

    private Comercio _currentSelectedShop;

    public UnityEvent OnRegisterSuccessful;
    public UnityEvent OnLoginSuccessful;

    #region Register
    
    [UsedImplicitly]
    public async void Register()
    {
        var body = new RegisterRequest
        {
            dni = _dniInputRegister.text,
            password = _passwordInputRegister.text
        };
        
        var json = JsonUtility.ToJson(body);
        Debug.Log("JSON enviado: " + json);

        var result = await APIClient.Instance.Post<RegisterResponse>("usuarios/registrar", body, 
            error => Debug.Log($"Error {error}: Datos incompletos. Se requieren: dni y password"));

        if (!result.Success)
        {
            PopUpManager.Instance.ChangePopUpText(result.ErrorMessage);
            PopUpManager.Instance.ShowPopUp();
            return;
        }

        var response = result.Data;
        
        Debug.Log($"Register OK: {response.message} Usuario creado exitosamente");
        Debug.Log("Usuario: " + response.usuario.nombres + " " + response.usuario.apellido);
        
        SaveUserDataToPlayerPrefs(response.usuario);
        LoadCredential();
            
        _backButtonHandler.GoToPanel(_credentialPanel);
        
        OnRegisterSuccessful?.Invoke();
    }
    #endregion

    #region Log In

    [UsedImplicitly]
    public async void Login()
    {
        var body = new LoginRequest()
        {
            dni = _dniInputLogin.text,
            password = _passwordInputLogin.text,
        };

        var result = await APIClient.Instance.Post<LoginResponse>("usuarios/login", body,
            err => Debug.LogError("Error Login: " + err)
        );


        if (!result.Success)
        {
            PopUpManager.Instance.ChangePopUpText(result.ErrorMessage);
            PopUpManager.Instance.ShowPopUp();
            return;
        }

        var response = result.Data;

        if (!string.IsNullOrEmpty(response.usuario.foto_perfil))
        {
            StartCoroutine(LoadProfileImage(response.usuario.foto_perfil));
        }
        else
        {
            Debug.Log("Usuario sin foto de perfil");
        }
        
        Debug.Log("Login OK");
        Debug.Log($"Response {response.usuario}");
        Debug.Log("Usuario: " + response.usuario.nombres + " " + response.usuario.apellido);
        Debug.Log("DNI: " + response.usuario.dni);

        SaveUserDataToPlayerPrefs(response.usuario);
        LoadCredential();
        _backButtonHandler.GoToPanel(_credentialPanel);
        
        OnLoginSuccessful?.Invoke();
    }
    #endregion

    #region Shops
    
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
    
    
    private List<DiscountUIElement> _loadedShops = new ();

    
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

        ClearShops();
        
        foreach (var shop in response.records)
        {
            Debug.Log($"{shop.nombre} / {shop.categoria}");
            
            var discounts = await LoadPromotions(shop.id);
            if (discounts.Count <= 0)
            {
                var uiElement = Instantiate(_discountsPrefab,_discountsContent.transform);
                uiElement.Initialize(shop, "No hay descuentos por el momento");
                _loadedShops.Add(uiElement);
                continue;
            }
            
            foreach (var discount in discounts)
            {
                var uiElement = Instantiate(_discountsPrefab, _discountsContent.transform);
                
                if(shop.horarios == null || shop.horarios.Count <= 0)
                    uiElement.Initialize(shop, discount,"Esta cerrado en este momento");
                else
                    uiElement.Initialize(shop, discount);

                uiElement.OnClick += (comercio, promocion) =>
                {
                    Debug.Log("CLICKINGNGNG");
                    _selectedShopName.text = comercio.nombre;
                    _selectedShopDescription.text = $"{comercio.direccion} \nHoy: {ScheduleUtils.GetTodayScheduleText(comercio)}" ;
                    _selectedShopDiscount.text = promocion.descuento + "%";
                    _selectedShopContainer.SetActive(true);
                    _currentSelectedShop = comercio;
                    hidePanelEvent?.Invoke();
                    _backButtonHandler.GoToPanel(_locationPanel);
                };
                _loadedShops.Add(uiElement);
            }

        }
    }

    void ClearShops()
    {
        if (_loadedShops.Count <= 0) 
            return;
        
        foreach (var loadedShop in _loadedShops)
        {
            Destroy(loadedShop.gameObject);
        }
        _loadedShops.Clear();
    }
    #endregion

    #region Promotions
    
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
    
    public async Task<List<Promocion>> LoadPromotions(int shopID)
    {
        var response = await APIClient.Instance.Get<PromocionesResponse>($"promociones/comercio/{shopID}");

        if (response?.records == null)
        {
            Debug.LogError("No Promotions");
            return null;
        }

        foreach (var discount in response.records)
        {
            Debug.Log($"{discount.titulo} ({discount.comercio_nombre})");
        }

        return response.records;
    }
    #endregion

    
    public void OpenSelectedShopInMaps()
    {
        if (_currentSelectedShop == null)
        {
            Debug.LogError("No shop selected");
            return;
        }
        
        MapsUtils.OpenGoogleMaps(_currentSelectedShop.latitud,_currentSelectedShop.longitud);
    }
    
    void SaveUserDataToPlayerPrefs(UserData userData)
    {
        PlayerPrefs.SetString("name", userData.nombres);
        PlayerPrefs.SetString("lastname", userData.apellido);
        PlayerPrefs.SetString("dni", userData.dni);
        PlayerPrefs.SetString("fecha_nacimiento", userData.fecha_nacimiento);
        PlayerPrefs.Save();
    }

    void LoadCredential()
    {
        Debug.Log("Credential");
        _credentialName.text = PlayerPrefs.GetString("name") + " " + PlayerPrefs.GetString("lastname");
        _credentialID.text = PlayerPrefs.GetString("dni");
        
        var birthDate = PlayerPrefs.GetString("fecha_nacimiento");
        _credentialBirthdate.text = string.IsNullOrEmpty(birthDate) ? "" : birthDate;
    }

    public IEnumerator LoadProfileImage(string url)
    {
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error loading profile image: {request.error}");
            yield break;
        }

        var tex = DownloadHandlerTexture.GetContent(request);

        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        _profileImage.sprite = sprite;
    }

    #region Show Hide Panel
    
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
    #endregion


}
