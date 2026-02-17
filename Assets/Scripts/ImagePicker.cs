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
            if (filePath == null) 
                return;

            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            UploadProfile(imageBytes);
            
            Texture2D texture2D = NativeGallery.LoadImageAtPath(filePath, imageSize);
            if (texture2D == null)
            {
                Debug.Log($"Couldn't load texture from {filePath}");
                return;
            }

            uiImage.texture = texture2D;
            uiImage.SetNativeSize();

        }, "Select a profile picture", "image/*");
    }

    public async void UploadProfile(byte[] imageBytes)
    {
        int id;
        if (!int.TryParse(PlayerPrefs.GetString("id"), out id))
        {
            return;
        }
        
        var response = await APIClient.Instance.UploadProfilePhoto(id, imageBytes);

        if (response != null)
        {
            Debug.Log("Foto actualizada");
        }
        else
        {
            Debug.LogError("Foto no pudo ser actualizada");
        }
    }
}
