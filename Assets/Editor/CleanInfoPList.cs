#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class CleanInfoPList
{
    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = pathToBuiltProject + "/Info.plist";
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Claves a ELIMINAR (las que la app NO usa):
        string[] remove = {
            "NSMicrophoneUsageDescription",
            "NSUserTrackingUsageDescription",
        };
        foreach (var key in remove)
            plist.root.values.Remove(key);

        // Si SÍ usas alguna, en vez de borrarla, escribe el texto correcto:
        // plist.root.SetString("NSCameraUsageDescription",
        //     "FAERAC usa la cámara para adjuntar una foto de tu comprobante al reservar un turno.");

        plist.WriteToFile(plistPath);
    }
}

#endif