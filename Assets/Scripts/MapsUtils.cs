using UnityEngine;

public static class MapsUtils
{
    public static void OpenGoogleMaps(string lat, string lng)
    {
        if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
        {
            Debug.LogError("Invalid Coordinates");
            return;
        }

        lat = lat.Replace(",", ".");
        lng = lng.Replace(",", ".");

        var url = $"https://www.google.com/maps/search/?api=1&query={lat},{lng}";
        
        Debug.Log($"Opening Maps: {url}");
        Application.OpenURL(url);
    }
}
