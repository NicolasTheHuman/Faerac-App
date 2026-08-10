using UnityEngine;
using UnityEngine.UI;

public class NotificationTestButton : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button == null) return;
        button.onClick.AddListener(OnClickTest);
    }

    private void OnClickTest()
    {
        NotificationService.Instance.FireTestNotifications();
    }
}
