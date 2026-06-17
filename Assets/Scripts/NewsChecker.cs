using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class NoticiaResponse
{
    public bool    success;
    public Noticia noticia;
}

[Serializable]
public class Noticia
{
    public int    id;
    public string titulo;
    public string cuerpo;
}

/// <summary>
/// Al iniciar consulta el endpoint GET noticias/ultima del servidor.
/// Si el ID de la noticia es mayor al último visto, muestra una notificación.
///
/// El backend debe responder:
///   { "success": true, "noticia": { "id": 5, "titulo": "...", "cuerpo": "..." } }
/// Si no hay novedades nuevas: { "success": false } o { "success": true, "noticia": null }
/// </summary>
public class NewsChecker : MonoBehaviour
{
    private const string PREFS_LAST_ID = "LastNoticiaId";

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        yield return CheckNews();
    }

    private IEnumerator CheckNews()
    {
        using var req = UnityWebRequest.Get(APIClient.BASE_URL + "noticias/ultima");
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        NoticiaResponse resp;
        try { resp = JsonUtility.FromJson<NoticiaResponse>(req.downloadHandler.text); }
        catch { yield break; }

        if (resp == null || !resp.success || resp.noticia == null) yield break;

        int lastId = PlayerPrefs.GetInt(PREFS_LAST_ID, 0);
        if (resp.noticia.id <= lastId) yield break;

        NotificationService.Instance?.ShowNewsNotification(resp.noticia.titulo, resp.noticia.cuerpo);
        PlayerPrefs.SetInt(PREFS_LAST_ID, resp.noticia.id);
        PlayerPrefs.Save();
    }
}
