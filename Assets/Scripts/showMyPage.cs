using UnityEngine;

public class OpenWebsiteButton : MonoBehaviour
{
    // Esta función se llama desde el botón
    public string website;
    public void OpenWebsite()
    {
        Application.OpenURL(website);
    }
}