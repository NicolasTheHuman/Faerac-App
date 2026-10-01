using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class AppVersionResponse
{
    public AppPlatformVersion android;
    public AppPlatformVersion ios;
}

[Serializable]
public class AppPlatformVersion
{
    public string latest_version;
    public string minimum_version;
    public string store_url;
}

/// <summary>Consulta GET app/config y selecciona la configuración de la plataforma</summary>
public class VersionChecker : MonoBehaviour
{
    private const string PREFS_LAST_NOTIFIED = "NotifiedVersion";

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        yield return CheckVersion();
    }

    private IEnumerator CheckVersion()
    {
        using var req = UnityWebRequest.Get(APIClient.BASE_URL.TrimEnd('/') + "/app/config");
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        AppVersionResponse resp;
        try { resp = JsonUtility.FromJson<AppVersionResponse>(req.downloadHandler.text); }
        catch { yield break; }

#if UNITY_IOS
        var config = resp?.ios;
#elif UNITY_ANDROID
        var config = resp?.android;
#else
        yield break;
#endif
#if UNITY_IOS || UNITY_ANDROID
        if (config == null || string.IsNullOrEmpty(config.latest_version)) yield break;
        if (!IsNewer(config.latest_version, Application.version)) yield break;
        if (PlayerPrefs.GetString(PREFS_LAST_NOTIFIED) == config.latest_version) yield break;
        NotificationService.Instance?.ShowUpdateNotification(config.latest_version);
        PlayerPrefs.SetString(PREFS_LAST_NOTIFIED, config.latest_version);
        PlayerPrefs.Save();
#endif
    }

    private static bool IsNewer(string remote, string local)
    {
        try
        {
            var r = remote.Split('.');
            var l = local.Split('.');
            int len = Math.Max(r.Length, l.Length);
            for (int i = 0; i < len; i++)
            {
                int rv = i < r.Length ? int.Parse(r[i]) : 0;
                int lv = i < l.Length ? int.Parse(l[i]) : 0;
                if (rv > lv) return true;
                if (rv < lv) return false;
            }
            return false;
        }
        catch { return false; }
    }
}
