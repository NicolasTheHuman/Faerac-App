using UnityEngine;

public class AndroidScreenshotBlocker : MonoBehaviour
{


#if UNITY_ANDROID && !UNITY_EDITOR
        void Awake()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    const int FLAG_SECURE = 0x00002000;
                    window.Call("setFlags", FLAG_SECURE, FLAG_SECURE);
                }
            }));
        }
    }
#endif


}
