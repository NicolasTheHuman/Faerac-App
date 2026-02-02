using System;
using UnityEngine;

[Serializable]
public class UploadPhotoResponse
{
    public string message;
    public UploadedPhoto foto_perfil;
}

[Serializable]
public class UploadedPhoto
{
    public string filename;
    public string url;
    public int size;
}
