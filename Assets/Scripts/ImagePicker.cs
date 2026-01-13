using UnityEngine;
using UnityEngine.UI;

public class ImagePicker : MonoBehaviour
{
    public RawImage uiImage;
    public int imageSize = 512;
    
    public void OnPickImageButton()
    {
        NativeGallery.GetImageFromGallery((filePath) =>
        {
            Debug.Log($"Image path {filePath}");
            if (filePath != null)
            {                
                Texture2D texture2D = NativeGallery.LoadImageAtPath(filePath, imageSize);
                if (texture2D == null)
                {
                    Debug.Log($"Couldn't load texture from {filePath}");
                    return;
                }

                uiImage.texture = texture2D;
                uiImage.SetNativeSize();
            }

        }, "Select an profile picture", "image/*");
    }
}
