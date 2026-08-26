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
        var encodedMessage = UnityWebRequest.EscapeURL(_whatsappMessage);
        string url = "https://api.whatsapp.com/send?phone=" + phoneNumber + "&text=" + encodedMessage;
        Application.OpenURL(url);
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
