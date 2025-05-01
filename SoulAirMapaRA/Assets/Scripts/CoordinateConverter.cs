using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CoordinateConverter : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Configuración del Mapa")]
    public Vector2 minCoords = new Vector2(-76.5533f, 3.3109f); // Esquina inferior izquierda
    public Vector2 maxCoords = new Vector2(-76.4921f, 3.3977f); // Esquina superior derecha

    [Header("Control de Zoom UI")]
    [SerializeField] private Slider zoomSlider; // Referencia al Slider en la UI

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

    private float resultvalidation;

    void Start()
    {
        InitializeZoomSlider();
    }

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

    //Calcula las coordenadas del ZoomMapa
    public void CalculateVisibleBoundingBox(out Vector2 newMinCoords, out Vector2 newMaxCoords)
    {
        // Esquina inferior izquierda
        Vector2 bottomLeft = ConvertPixelsToCoordinates(new Vector2(
            (-720f - ((mapRectTransform.anchoredPosition.x) * 2)) * 0.5f,
            (-1280f - ((mapRectTransform.anchoredPosition.y) * 2)) * 0.5f
        ));

        // Esquina superior derecha
        Vector2 topRight = ConvertPixelsToCoordinates(new Vector2(
            (720f - ((mapRectTransform.anchoredPosition.x) * 2)) * 0.5f,
            (1280f - ((mapRectTransform.anchoredPosition.y) * 2)) * 0.5f
        ));

        newMinCoords = bottomLeft;
        newMaxCoords = topRight;


    }

    //Actualiza el nuevo mapa si se movio o si se hizo zoom
    public void updateZoomMap(bool move)
    {
        if (sizeMap != lastMapSizeNumber || move)
        {
            CalculateVisibleBoundingBox(out visibleMin, out visibleMax);
            mapaZoom.SetActive(true);
            mapBoxZoom.minLatitude = visibleMin.y;
            mapBoxZoom.maxLatitude = visibleMax.y;
            mapBoxZoom.minLongitude = visibleMin.x;
            mapBoxZoom.maxLongitude = visibleMax.x;
        }
    }

    private void InitializeZoomSlider()
    {
        if (zoomSlider != null)
        {
            // Configurar rango del slider según los límites del zoom
            zoomSlider.minValue = 1.0f;
            zoomSlider.maxValue = 4.0f;
            zoomSlider.value = 1.0f;

            // Asignar el listener para detectar cambios
            zoomSlider.onValueChanged.AddListener(OnZoomSliderChanged);
        }
        else
        {
            Debug.LogWarning("Slider de zoom no asignado en el inspector");
        }
    }

    public void OnZoomSliderChanged(float newZoomValue)
    {
        // Aplicar el nuevo valor de zoom respetando los límites
        sizeMap = Mathf.Clamp(newZoomValue, 0.9f, 4.1f);
    }

    public void resetZoom()
    {
        mapRectTransform.anchoredPosition = new Vector2(0f, 66.123f);
        sizeMap = 1.0f;
        zoomSlider.value = 1.0f;
    }

    public void validacionLimites()
    {
        // Obtener dimensiones actuales
        float mapWidth = mapRectTransform.rect.width;
        float mapHeight = mapRectTransform.rect.height;
        Vector2 currentPos = mapRectTransform.anchoredPosition;

        // Obtener tamaño del canvas (asumiendo que es el padre directo)
        RectTransform canvasRect = mapRectTransform.parent.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Calcular límites permitidos
        float leftLimit = (canvasWidth - mapWidth) * 0.5f;
        float rightLimit = -leftLimit;
        float bottomLimit = (canvasHeight - mapHeight) * 0.5f;
        float topLimit = -bottomLimit;

        // Variables para nueva posición
        float newX = currentPos.x;
        float newY = currentPos.y;

        // Validar límites horizontales
        if (mapWidth > canvasWidth)
        {
            // Mapa más ancho que el canvas - centrar horizontalmente
            newX = 0;
        }
        else
        {
            // Ajustar si se sale por izquierda o derecha
            newX = Mathf.Clamp(currentPos.x, rightLimit, leftLimit);
        }

        // Validar límites verticales
        if (mapHeight > canvasHeight)
        {
            // Mapa más alto que el canvas - centrar verticalmente
            newY = 0;
        }
        else
        {
            // Ajustar si se sale por arriba o abajo
            newY = Mathf.Clamp(currentPos.y, topLimit, bottomLimit);
        }

        // Aplicar nueva posición si es necesario
        if (newX != currentPos.x || newY != currentPos.y)
        {
            mapRectTransform.anchoredPosition = new Vector2(newX, newY);

        }

        sizeMap = lastMapSizeNumber += 0.1f;
        zoomSlider.value = lastMapSizeNumber += 0.1f;
    }

}
