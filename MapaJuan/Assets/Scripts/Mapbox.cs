using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Globalization;

public class Mapbox : MonoBehaviour
{
    //Variables para modificar el mapa desde el inspector 
    public string accessToken;
    public float minLatitude = 3.3109f;
    public float minLongitude = -76.5533f;
    public float maxLatitude = 3.3977f;
    public float maxLongitude = -76.4921f;


    public Vector2 currentGPS = new Vector2(-76.52081f, 3.35117f); // [lon, lat]
    public string avatarUrl;


    //Estilos del Mapa desde el inspector
    public enum style { Light, Dark, Streets, Outdoors, Satellite, SatelliteStreets };
    public style mapStyle = style.Streets;

    //Resolucion del mapa desde el inspector
    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.high;

    //Control and Set the map and the execution 
    private int mapWidth = 720;
    private int mapHeight = 1280;
    private string[] styleStr = new string[] { "light-v10", "dark-v10", "streets-v12", "outdoors-v11", "satellite-v9", "satellite-streets-v11" };
    private string url = "";
    public bool mapIsLoading = false;
    private Rect rect;
    private bool updateMap = true;

    //Datos anteriores guardados
    private string accessTokenLast;
    private float minLatitudeLast = 3.3109f;
    private float minLongitudeLast = -76.5533f;
    private float maxLatitudeLast = 3.3977f;
    private float maxLongitudeLast = -76.4921f;
    private style mapStyleLast = style.Streets;
    private resolution mapResolutionLast = resolution.high;





    void Start()
    {
        //Tamaño de la imagen
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        StartCoroutine(GetMapbox());
    }

    void Update()
    {
        

        
        if (updateMap && (accessTokenLast != accessToken ||
            !Mathf.Approximately(minLatitudeLast, minLatitude) ||
            !Mathf.Approximately(minLongitudeLast, minLongitude) ||
            !Mathf.Approximately(maxLatitudeLast, maxLatitude) ||
            !Mathf.Approximately(maxLongitudeLast, maxLongitude) ||
            mapStyleLast != mapStyle ||
            mapResolutionLast != mapResolution))
        {
            StartCoroutine(GetMapbox());
            updateMap = false;
            

        }
    }
    

    private bool IsWithinCoverageArea(Vector2 coordinates)
    {
        return coordinates.x >= minLongitude &&
               coordinates.x <= maxLongitude &&
               coordinates.y >= minLatitude &&
               coordinates.y <= maxLatitude;
    }

    //Consulta a la API
    IEnumerator GetMapbox()
    {

        if (!IsWithinCoverageArea(currentGPS))
        {
            Debug.LogWarning("¡Estás por fuera de la zona de cobertura!");

        }
        

        string encodedAvatarUrl = UnityWebRequest.EscapeURL(avatarUrl);
        string avatar = $"url-{encodedAvatarUrl}({currentGPS.x.ToString(CultureInfo.InvariantCulture)},{currentGPS.y.ToString(CultureInfo.InvariantCulture)})";



        string bbox = $"{minLongitude.ToString(CultureInfo.InvariantCulture)}," +
              $"{minLatitude.ToString(CultureInfo.InvariantCulture)}," +
              $"{maxLongitude.ToString(CultureInfo.InvariantCulture)}," +
              $"{maxLatitude.ToString(CultureInfo.InvariantCulture)}";


        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] +
              "/static/" + $"{avatar}/" + $"[{bbox}]" + "/" +
              mapWidth + "x" + mapHeight +
              (mapResolution == resolution.high ? "@2x" : "") +
              "?access_token=" + accessToken +
              "&logo=false" +
              "&attribution=false";

        Debug.Log("Requesting map: " + url); // Para depuración

        mapIsLoading = true;
 
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Mapbox API Error: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
            updateMap = true;

        }
        else
        {
            mapIsLoading = false;

            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            minLatitudeLast = minLatitude;
            maxLatitudeLast = maxLatitude;
            minLongitudeLast = minLongitude;
            maxLongitudeLast = maxLongitude;
            accessTokenLast = accessToken;
            mapStyleLast = mapStyle;
            mapResolutionLast = mapResolution;
            updateMap = true;

        }
    }


    
}