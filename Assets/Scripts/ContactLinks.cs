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
        var encodedMessage = UnityWebRequest.EscapeURL(_whatsappMessage).Replace("+", "%20");
        Application.OpenURL($"https://wa.me/{_whatsappNumber}?text={encodedMessage}");
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
