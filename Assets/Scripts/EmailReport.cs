using UnityEngine;
using UnityEngine.Networking;

public class EmailReport : MonoBehaviour
{
    [SerializeField] private string _subject = "Feedback App";
    [TextArea] [SerializeField] private string _body = "Please enter your message here.";
    
    private const string RecipientEmail = "administracion@faerac.org.ar";

    public void SendEmail()
    {
        var escapedSubject = EscapeURL(_subject);
        var escapedBody = EscapeURL(_body);

        var mailToUrl = $"mailto:{RecipientEmail}?subject={escapedSubject}&body={escapedBody}";
        
        Application.OpenURL(mailToUrl);
    }

    private string EscapeURL(string url) => UnityWebRequest.EscapeURL(url).Replace("+", "%20");
}
