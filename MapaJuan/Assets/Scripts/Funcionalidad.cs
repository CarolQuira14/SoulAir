// Requiere estos namespaces:
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Globalization;
using System.Text;

public class Funcionalidad : MonoBehaviour
{
    
    [Header("Mapbox")]
    public string accessToken = "TU_ACCESS_TOKEN";
    public RawImage mapDisplay;

    [Header("GPS Simulado")]
    public Vector2 currentGPS = new Vector2(-76.52081f, 3.35117f); // [lon, lat]
    public float gpsRadius = 0.001f; // Radio en grados

    private Texture2D mapTexture;
    private string mapUrl;

    private Vector2 bboxMin = new Vector2(-76.5791f, 3.3371f);  // [lon_min, lat_min]
    private Vector2 bboxMax = new Vector2(-76.4826f, 3.3853f);  // [lon_max, lat_max]

    void Start()
    {
        StartCoroutine(UpdateMap());
    }

   

    IEnumerator UpdateMap()
    {
        // 1. Configurar parámetros básicos
        string username = "mapbox"; // Usuario por defecto de Mapbox
        string styleId = "streets-v12"; // Estilo del mapa


        // 3. Configurar marcador GPS (círculo)
        string markerColor = "ff0000"; // Rojo
        string overlay = $"pin-s+{markerColor}({currentGPS.x.ToString(CultureInfo.InvariantCulture)},{currentGPS.y.ToString(CultureInfo.InvariantCulture)})";

        // 4. Construir URL correctamente
        string mapUrl = $"https://api.mapbox.com/styles/v1/{username}/{styleId}/static/" +
                       $"{overlay}/" +
                       $"[{bboxMin.x},{bboxMin.y},{bboxMax.x},{bboxMax.y}]/" +
                       $"400x400" +
                       $"?access_token={accessToken}" +
                       $"&logo=false" +
                       $"&attribution=false";

        // 5. Descargar el mapa
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mapUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            mapTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            mapDisplay.texture = mapTexture;
        }
        else
        {
            Debug.LogError($"Error: {request.error}\nURL: {mapUrl}\nResponse: {request.downloadHandler.text}");
        }
    }

    

}
