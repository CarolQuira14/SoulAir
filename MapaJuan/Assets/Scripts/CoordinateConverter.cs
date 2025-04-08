using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoordinateConverter : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Configuración del Mapa")]
    public Vector2 minCoords = new Vector2(-76.5533f, 3.3109f); // Esquina inferior izquierda
    public Vector2 maxCoords = new Vector2(-76.4921f, 3.3977f); // Esquina superior derecha

    [Header("Referencias")]
    public RectTransform mapRectTransform;
    public float sizeMap = 1.0f;
    public float lastMapSizeNumber;

    public Vector2 localPosition;

    public float currentWidth;
    public float currentHeight;

    public Vector2 visibleMin, visibleMax;

    public GameObject mapaZoom;
    public Mapbox mapBoxZoom;

    

    // Convierte coordenadas geográficas a posición local en el mapa
    public Vector2 ZoomMapPositionParticles(Vector2 gpsCoords)
    {
        if (sizeMap > 0.9f && sizeMap < 4.1f)
        {
            mapRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeMap * 720f);
            mapRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeMap * 1280f);
            ConvertCoordinatestoPixels(gpsCoords);
            updateZoomMap(false);



            if (sizeMap == 1.0f)
            {
                mapaZoom.SetActive(false);
            }

            Debug.Log($"Tamaño actual del mapa: {mapRectTransform.rect.size} - {mapRectTransform.rect.x} - {mapRectTransform.rect.y}");

            return localPosition;
        }
        else
        {
            Debug.Log($"Exediste de Zoom: {mapRectTransform.rect.size}");

            return localPosition;
        }



    }

    public Vector2 ConvertPixelsToCoordinates(Vector2 pixelPosition)
    {
        // Obtener posición normalizada (0-1) dentro del mapa
        float xNormalized = (pixelPosition.x + mapRectTransform.rect.width * 0.5f) / mapRectTransform.rect.width;
        float yNormalized = (pixelPosition.y + mapRectTransform.rect.height * 0.5f) / mapRectTransform.rect.height;

        // Calcular coordenadas geográficas
        Vector2 geoCoords = new Vector2(
            Mathf.Lerp(minCoords.x, maxCoords.x, xNormalized),
            Mathf.Lerp(minCoords.y, maxCoords.y, yNormalized)
        );

        Debug.Log(geoCoords);

        return geoCoords;
    }

    public Vector2 ConvertCoordinatestoPixels(Vector2 gpsCoords)
    {

        // Normalizar las coordenadas (0-1)
        float xNormalized = Mathf.InverseLerp(minCoords.x, maxCoords.x, gpsCoords.x);
        float yNormalized = Mathf.InverseLerp(minCoords.y, maxCoords.y, gpsCoords.y);

        // Calcular posición en pixels relativos al tamaño actual

        currentWidth = mapRectTransform.rect.width;
        currentHeight = mapRectTransform.rect.height;


        // Convertir a coordenadas locales del RectTransform
        localPosition = new Vector2(
            xNormalized * currentWidth - currentWidth * 0.5f,
            yNormalized * currentHeight - currentHeight * 0.5f
        );

        return localPosition;
    }

    public void CalculateVisibleBoundingBox(out Vector2 newMinCoords, out Vector2 newMaxCoords)
    {
        // Esquina inferior izquierda
        Vector2 bottomLeft = ConvertPixelsToCoordinates(new Vector2(
            (-720f -((mapRectTransform.anchoredPosition.x)*2)) * 0.5f,
            (-1280f - ((mapRectTransform.anchoredPosition.y)*2)) * 0.5f
        ));

        // Esquina superior derecha
        Vector2 topRight = ConvertPixelsToCoordinates(new Vector2(
            (720f - ((mapRectTransform.anchoredPosition.x) * 2)) * 0.5f,
            (1280f - ((mapRectTransform.anchoredPosition.y)*2)) * 0.5f
        ));

        newMinCoords = bottomLeft;
        newMaxCoords = topRight;

        
    }

    public void updateZoomMap(bool move)
    {
        if (sizeMap != lastMapSizeNumber || move)
        {
            CalculateVisibleBoundingBox(out visibleMin, out visibleMax);
            Debug.Log(visibleMin);
            Debug.Log(visibleMax);

            mapaZoom.SetActive(true);
            mapBoxZoom.minLatitude = visibleMin.y;
            mapBoxZoom.maxLatitude = visibleMax.y;
            mapBoxZoom.minLongitude = visibleMin.x;
            mapBoxZoom.maxLongitude = visibleMax.x;
        }
    }
}
