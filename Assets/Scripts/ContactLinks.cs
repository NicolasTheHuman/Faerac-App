using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ContactLinks : MonoBehaviour
{
    [Header("WhatsApp")]
    [SerializeField] private string _whatsappNumber = "5491100000000";
    [SerializeField] private string _whatsappMessage = "Hola, me comunico desde la app Faerac.";

    [Header("Email")]
    [SerializeField] private string _emailRecipient = "administracion@faerac.org.ar";
    [SerializeField] private string _emailSubject = "Consulta desde la App";
    [TextArea] [SerializeField] private string _emailBody = "";

    [Header("Ubicación")]
    [SerializeField] private string _latitude = "-34.6037";
    [SerializeField] private string _longitude = "-58.3816";

    public void OpenWhatsApp()
    {
        OpenWhatsAppNumber(_whatsappNumber);
    }

    public void OpenWhatsAppNumber(string phoneNumber)
    {
        StartCoroutine(OpenWhatsAppChat(phoneNumber, _whatsappMessage));
    }

    // Abre el chat directamente en la app de WhatsApp. Si la app no está
    // instalada (no salimos de nuestra app), cae al link web.
    public static IEnumerator OpenWhatsAppChat(string phoneNumber, string message = null)
    {
        var number = NormalizeWhatsAppNumber(phoneNumber);
        if (string.IsNullOrEmpty(number))
        {
            Debug.LogError("Invalid WhatsApp number: " + phoneNumber);
            yield break;
        }

        var query = "send?phone=" + number;
        if (!string.IsNullOrEmpty(message))
            query += "&text=" + UnityWebRequest.EscapeURL(message).Replace("+", "%20");

        bool leftApp = false;
        Action<bool> onFocusChanged = focused => { if (!focused) leftApp = true; };
        Application.focusChanged += onFocusChanged;

        Application.OpenURL("whatsapp://" + query);
        yield return new WaitForSecondsRealtime(1.5f);

        Application.focusChanged -= onFocusChanged;
        if (!leftApp)
            Application.OpenURL("https://api.whatsapp.com/" + query);
    }

    // WhatsApp necesita el número en formato internacional, solo dígitos.
    // Celulares de Argentina: 54 + 9 + código de área + número (sin 0 ni 15).
    public static string NormalizeWhatsAppNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return "";

        var digits = new System.Text.StringBuilder();
        foreach (var c in phoneNumber)
        {
            if (c >= '0' && c <= '9') digits.Append(c);
        }

        var number = digits.ToString().TrimStart('0');

        if (number.Length == 10) return "549" + number;
        if (number.Length == 12 && number.StartsWith("54")) return "549" + number.Substring(2);
        return number;
    }

    public void CallPhone(string phoneNumber)
    {
        Application.OpenURL("tel:" + phoneNumber);
    }

    public void OpenEmail()
    {
        var subject = UnityWebRequest.EscapeURL(_emailSubject).Replace("+", "%20");
        var body = UnityWebRequest.EscapeURL(_emailBody).Replace("+", "%20");
        Application.OpenURL($"mailto:{_emailRecipient}?subject={subject}&body={body}");
    }

    public void OpenMaps()
    {
        MapsUtils.OpenGoogleMaps(_latitude, _longitude);
    }
}
