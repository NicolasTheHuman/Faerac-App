using UnityEngine;

public class OpenWebsiteButton : MonoBehaviour
{
    // Esta funci�n se llama desde el bot�n
    public string website;
    public string androidWebsite;
    public string iosWebsite;

    public void OpenWebsite()
    {
        string url = website;

#if UNITY_ANDROID
        if (!string.IsNullOrEmpty(androidWebsite))
            url = androidWebsite;
#elif UNITY_IOS
        if (!string.IsNullOrEmpty(iosWebsite))
            url = iosWebsite;
#endif

        Application.OpenURL(url);
    }
}