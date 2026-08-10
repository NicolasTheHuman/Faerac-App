using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionNumberDisplay : MonoBehaviour
{
    [SerializeField] private string format = "v{0}";

    private void Awake()
    {
        GetComponent<TMP_Text>().text = string.Format(format, Application.version);
    }
}
