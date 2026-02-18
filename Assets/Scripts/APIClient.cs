using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
    
public class APIClient : MonoBehaviour
{
    public static APIClient Instance { get; private set; }

    public const string BASE_URL = "https://aconcaguastudios.com/faerac-api/";
    private const string CONTENT_TYPE = "application/json";
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<T> Get<T>(string endpoint, Action<string> onError = null)
    {
        var request = UnityWebRequest.Get($"{BASE_URL}{endpoint}");
        Debug.Log($"GET URL: {BASE_URL}{endpoint}");

        request.SetRequestHeader("Content-Type", CONTENT_TYPE);
        request.timeout = 10;

        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        
        Debug.Log("RAW RESPONSE: " + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            Debug.LogError($"GET Error {request.responseCode}: {request.error}");
            return default;
        }

        try
        {
            return JsonUtility.FromJson<T>(request.downloadHandler.text);
        }
        catch (Exception e)
        {
            onError?.Invoke("JSON Parse Error: " + e.Message);
            return default;
        }
    }

    public async Task<ApiResult<T>> Post<T>(string endpoint, object body, Action<string> onError = null)
    {
        var url = $"{BASE_URL}{endpoint}";
        Debug.Log("POST URL: " + url);

        var json = JsonConvert.SerializeObject(body);
        Debug.Log("JSON enviado: " + json);
        var jsonBytes = Encoding.UTF8.GetBytes(json);
        
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", CONTENT_TYPE);
        request.timeout = 10;

        await request.SendWebRequest();


        Debug.Log("HTTP result: " + request.result);
        Debug.Log("HTTP code: " + request.responseCode);
        Debug.Log("Respuesta cuerpo: " + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
        {
            //onError?.Invoke($"HTTP Error {request.responseCode}: {request.error}");
            //return default;

            var errorMessage = "Unkown server error.";

            try
            {
                var error = JsonConvert.DeserializeObject<ApiErrorResponse>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(error.message))
                    errorMessage = error.message;
            }
            catch (Exception e)
            {
                errorMessage = request.downloadHandler.text;
            }

            return ApiResult<T>.Fail(errorMessage, request.responseCode);
        }

        //return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        try
        {
            var data = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            return ApiResult<T>.Ok(data, request.responseCode);
        }
        catch (Exception e)
        {
            return ApiResult<T>.Fail("Invalid server response.", request.responseCode);
        }
    }
    
    public async Task<T> PostUrlEncoded<T>(string endpoint, Dictionary<string, string> fields, Action<string> onError = null)
    {
        string url = $"{BASE_URL}{endpoint}";
        Debug.Log("POST URL: " + url);

        // Construir cuerpo x-www-form-urlencoded
        string formBody = "";
        foreach (var f in fields)
            formBody += $"{UnityWebRequest.EscapeURL(f.Key)}={UnityWebRequest.EscapeURL(f.Value)}&";
        formBody = formBody.TrimEnd('&');

        byte[] bodyRaw = Encoding.UTF8.GetBytes(formBody);

        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        var operation = request.SendWebRequest();
        while (!operation.isDone) await Task.Yield();

        Debug.Log("HTTP Result: " + request.result);
        Debug.Log("HTTP Code: " + request.responseCode);
        Debug.Log("Response: " + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            return default;
        }

        try
        {
            return JsonUtility.FromJson<T>(request.downloadHandler.text);
        }
        catch (Exception e)
        {
            onError?.Invoke("JSON Parse Error: " + e.Message);
            return default;
        }
    }

    public async Task<UploadPhotoResponse> UploadProfilePhoto(int userID, byte[] imageBytes, string fileName = "profile.jpg")
    {
        var url = $"{BASE_URL}usuarios/{userID}/foto-perfil";
        Debug.Log($"Upload URL: {url}");

        // Crear manualmente el cuerpo multipart para evitar que Unity agregue encabezados Expect/fragmentados que Apache rechaza
        string boundary = "----UnityBoundary" + System.DateTime.Now.Ticks.ToString("x");
        byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        byte[] endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        byte[] headerBytes = Encoding.ASCII.GetBytes(
            $"Content-Disposition: form-data; name=\"foto\"; filename=\"{fileName}\"\r\nContent-Type: image/jpeg\r\n\r\n");

        var body = new List<byte>();
        body.AddRange(Encoding.ASCII.GetBytes("--" + boundary + "\r\n"));
        body.AddRange(headerBytes);
        body.AddRange(imageBytes);
        body.AddRange(endBoundaryBytes);

        byte[] bodyBytes = body.ToArray();

        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.chunkedTransfer = false;
        request.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={boundary}");
        request.SetRequestHeader("Content-Length", bodyBytes.Length.ToString());
        await request.SendWebRequest();

        var responseText = request.downloadHandler.text;
        Debug.Log($"UPLOAD RESPONSE: {responseText}");

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Upload error {request.responseCode}: {request.error}");
            return null;
        }

        return JsonConvert.DeserializeObject<UploadPhotoResponse>(responseText);
    }
}
