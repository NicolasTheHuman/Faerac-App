using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class AppVersionResponse
{
    public bool   success;
    public string version;
}

/// <summary>
/// Al iniciar compara la versión actual contra el endpoint GET app/version
/// del servidor. Si hay una versión más nueva muestra una notificación.
///
/// El backend debe responder: { "success": true, "version": "1.2.0" }
/// </summary>
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
        using var req = UnityWebRequest.Get(APIClient.BASE_URL + "app/version");
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        AppVersionResponse resp;
        try { resp = JsonUtility.FromJson<AppVersionResponse>(req.downloadHandler.text); }
        catch { yield break; }

        if (resp == null || !resp.success || string.IsNullOrEmpty(resp.version)) yield break;
        if (!IsNewer(resp.version, Application.version)) yield break;
        if (PlayerPrefs.GetString(PREFS_LAST_NOTIFIED) == resp.version) yield break;

        NotificationService.Instance?.ShowUpdateNotification(resp.version);
        PlayerPrefs.SetString(PREFS_LAST_NOTIFIED, resp.version);
        PlayerPrefs.Save();
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
