using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using System.Collections.Generic;
using System;
using System.Linq;

public class HereTrafficAPI : MonoBehaviour
{
    [Header("Configuración API HERE")]
    [SerializeField] private string apiKey = "TU_API_KEY"; // ¡CAMBIAR ESTO POR TU API KEY REAL!
    private string apiUrl = "https://data.traffic.hereapi.com/v7/flow";
    [Tooltip("Bounding Box para la consulta de tráfico. Formato: lon_oeste,lat_sur,lon_este,lat_norte")]
    [SerializeField] private string bboxZonaValleDelLili = "-76.5358,3.3693,-76.5352,3.3697";

    [Header("Referencias de Escena")]
    [Tooltip("Referencia al conversor de coordenadas GPS a píxeles.")]
    public CoordinateConverter coordinateConverter;
    [Tooltip("Prefab para instanciar los segmentos de calle (debe tener un LineRenderer).")]
    public GameObject streetLinePrefab;
    [Tooltip("Contenedor donde se instanciarán los objetos de línea de calle.")]
    public Transform mapboxContainer;
    [Tooltip("ICA Promedio")]
    public AirQualityCalculator ICA;
    [Header("Prefabs Adicionales")]
    [Tooltip("Prefab a instanciar en puntos medios bajo ciertas condiciones de tráfico y ICA.")]
    public GameObject alertPrefab; // Arrastra tu prefab aquí en el Inspector
    [Tooltip("Contenedor donde se instanciarán los PopUps")]
    public Transform popUpsContainer;

    [Header("Configuración de Colores de Tráfico")]
    [Tooltip("Color para tráfico fluido (JamFactor 0-3.9)")]
    public Color colorLibre = new Color(0.0f, 0.8f, 0.0f);
    [Tooltip("Color para tráfico moderado (JamFactor 4.0-7.9)")]
    public Color colorModerado = new Color(1.0f, 0.65f, 0.0f);
    [Tooltip("Color para tráfico pesado (JamFactor 8.0-9.9)")]
    public Color colorPesado = new Color(0.8f, 0.0f, 0.0f);
    [Tooltip("Color para tráfico bloqueado o carretera cerrada (JamFactor 10.0)")]
    public Color colorBloqueado = new Color(0.5f, 0.0f, 0.0f);
    [Tooltip("Color por defecto si el JamFactor es inesperado")]
    public Color colorPorDefecto = Color.gray;

    // Lista para gestionar las líneas de tráfico creadas
    private List<LineRenderer> managedStreetLines = new List<LineRenderer>();
    private List<int> positionLineRender = new List<int>();
    //private List<ArrayList> posicione = new List<ArrayList>();
    private bool isInitialLoadAttempted = false;
    private bool isInitialLoadSuccessful = false;

    private Dictionary<LineRenderer, Vector3[]> originalPositions = new Dictionary<LineRenderer, Vector3[]>();

    // Intervalo de actualización de tráfico en segundos (15 minutos)
    private const float TRAFFIC_UPDATE_INTERVAL_SECONDS = 3 * 60;

    void Start()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "TU_API_KEY")
        {
            Debug.LogError("[HereTrafficAPI] API Key no configurada. Por favor, introduce tu API Key de HERE en el Inspector.");
            return;
        }
        if (coordinateConverter == null || streetLinePrefab == null || mapboxContainer == null)
        {
            Debug.LogError("[HereTrafficAPI] Faltan referencias esenciales (CoordinateConverter, StreetLinePrefab o MapboxContainer). Asígnalas en el Inspector.");
            return;
        }
        // Iniciar la primera carga de datos de tráfico
        StartCoroutine(FetchAndProcessTrafficData(bboxZonaValleDelLili, true));
        positionLineRender.Add(0);
    }

    

    /// Inicia el ciclo de actualizaciones periódicas de tráfico si la carga inicial fue exitosa.
    private void StartPeriodicUpdates()
    {

        if (isInitialLoadSuccessful && Application.isPlaying) // Asegurarse que el juego sigue corriendo
        {
            Debug.Log($"[HereTrafficAPI] Iniciando actualizaciones periódicas de tráfico cada {TRAFFIC_UPDATE_INTERVAL_SECONDS / 60} minutos.");
            StartCoroutine(PeriodicTrafficUpdateRoutine());
        }
        else if (!isInitialLoadSuccessful)
        {
            Debug.LogError("[HereTrafficAPI] No se iniciarán actualizaciones periódicas porque la carga inicial falló o no se completó.");
        }
    }

    /// <summary>
    /// Actualiza la posición y el tamaño de las líneas de calle basado en un factor de escala.
    /// Usa las posiciones originales para evitar el crecimiento exponencial.
    /// </summary>
    /// <param name="size">Factor que influye en la escala y el ancho de las líneas.</param>
    public void UpdatePosition(float size)
    {
        // El factor de escala para las posiciones.
        float positionScaleFactor = size - 0.25f;

        // Asegurarse de que el factor de escala no sea cero o negativo si no se desea.
        // Por ejemplo, podrías querer un mínimo: if (positionScaleFactor <= 0) positionScaleFactor = 0.01f;

        foreach (var linerender in managedStreetLines)
        {
            if (linerender == null || !linerender.gameObject.activeInHierarchy)
            {
                // Omitir si el LineRenderer es nulo o su GameObject está inactivo.
                continue;
            }

            if (originalPositions.TryGetValue(linerender, out Vector3[] initialPositions))
            {
                Vector3[] newPositions = new Vector3[initialPositions.Length];
                for (int i = 0; i < initialPositions.Length; i++)
                {
                    // Escalar desde las posiciones originales.
                    newPositions[i] = initialPositions[i] * positionScaleFactor;
                }
                linerender.SetPositions(newPositions);

                // Ajuste del ancho de la línea.
                // Se calcula basado en 'size' para que el ancho sea consistente con el 'size' actual.
                float baseWidth = 0.25f;
                float widthAdjustment = size - 1.5f; // Este es el ajuste que tenías.
                float finalWidth = baseWidth + widthAdjustment; // Resultado: size - 1.25f

                // Asegurar un ancho mínimo positivo para la línea.
                finalWidth = Mathf.Max(0.01f, finalWidth);

                linerender.startWidth = finalWidth;
                linerender.endWidth = finalWidth;
            }
            else
            {
                // Esto podría ocurrir si un LineRenderer en managedStreetLines no fue añadido a originalPositions.
                // Debería investigarse si sucede, pero es una precaución.
                Debug.LogWarning($"[HereTrafficAPI] No se encontraron posiciones originales para un LineRenderer. Se omitió el escalado para este objeto: {linerender.gameObject.name}");
            }
        }
    }
    /// Corrutina que gestiona las llamadas periódicas para actualizar los datos de tráfico.
    private IEnumerator PeriodicTrafficUpdateRoutine()
    {
        // Esperar el intervalo definido antes de la primera actualización periódica
        yield return new WaitForSeconds(TRAFFIC_UPDATE_INTERVAL_SECONDS);

        while (Application.isPlaying) // Continuar mientras el juego esté en ejecución
        {
            Debug.Log("[HereTrafficAPI] Solicitando actualización periódica de datos de tráfico...");
            // Llamar a FetchAndProcessTrafficData para una actualización (no es configuración inicial)
            yield return StartCoroutine(FetchAndProcessTrafficData(bboxZonaValleDelLili, false));
            // Esperar para la siguiente actualización
            yield return new WaitForSeconds(TRAFFIC_UPDATE_INTERVAL_SECONDS);
        }
    }


    private IEnumerator FetchAndProcessTrafficData(string bbox, bool isInitialSetup)
    {
        if (isInitialSetup)
        {
            isInitialLoadAttempted = true;
            isInitialLoadSuccessful = false; // Asumir fallo hasta que se complete exitosamente
        }
        string url = $"{apiUrl}?locationReferencing=shape&in=bbox:{bbox}&apiKey={apiKey}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[HereTrafficAPI] Datos de tráfico recibidos exitosamente.");
                var json = JSON.Parse(request.downloadHandler.text);
                var flows = json["results"];

                if (flows == null || flows.Count == 0)
                {
                    Debug.LogWarning("No se recibieron datos de tráfico para la BBOX especificada. Verifica la BBOX y tu API Key.");
                    yield break;
                }

                // Si es configuración inicial, limpiar líneas antiguas (por si se re-ejecuta)
                if (isInitialSetup)
                {
                    foreach (var lineRenderer in managedStreetLines)
                    {
                        if (lineRenderer != null) Destroy(lineRenderer.gameObject);
                    }
                    managedStreetLines.Clear();
                }
                else
                {
                    GameObject[] focosPolution = GameObject.FindGameObjectsWithTag("foco");
                    foreach (var foco in focosPolution)
                    {
                        if (foco != null) Destroy(foco.gameObject);
                    }
                }
                    

                int currentSegmentIndex = 0; // Índice para rastrear los segmentos de línea a través de múltiples 'items'
                int currentLinerenderIndex = 0; // Índice para rastrear los linerenders 

                foreach (var item in flows.Children)
                {
                    // Verificar si currentFlow y jamFactor existen antes de acceder a ellos
                    if (item["currentFlow"] == null || item["currentFlow"]["jamFactor"] == null)
                    {
                        Debug.LogWarning("Item de flujo de tráfico no contiene 'currentFlow' o 'jamFactor'. Saltando este item.");
                        continue;
                    }

                    float jamFactor = item["currentFlow"]["jamFactor"].AsFloat;
                    float jamTendency = item["currentFlow"]["jamTendency"]?.AsFloat ?? 0f; // Si no hay tendencia, asumir 0
                    Color trafficColor = GetColorByJamFactor(jamFactor, jamTendency); // El JF/JT del 'item' aplica a sus 'links'
                    var shape = item["location"]["shape"]["links"];
                    foreach (var link in shape.Children)
                    {

                        if (isInitialSetup)
                        {
                            var pointsNode = link["points"];
                            if (pointsNode == null || pointsNode.Count < 2)
                            {
                                Debug.LogWarning("[HereTrafficAPI] Link no contiene suficientes 'points' para dibujar. Saltando este link.");
                                continue;
                            }

                            List<Vector3> screenPoints = new List<Vector3>();
                            foreach (var point in pointsNode.Children)
                            {
                                float lat = point["lat"].AsFloat;
                                float lng = point["lng"].AsFloat;
                                Vector2 pixelPos = coordinateConverter.ConvertCoordinatestoPixels(new Vector2(lng, lat));
                                screenPoints.Add(new Vector3(pixelPos.x, pixelPos.y, 0));
                            }

                            GameObject newLineGO = Instantiate(streetLinePrefab, mapboxContainer);
                            LineRenderer lineRenderer = newLineGO.GetComponent<LineRenderer>();

                            if (lineRenderer != null)
                            {
                                lineRenderer.positionCount = screenPoints.Count;
                                lineRenderer.useWorldSpace = false; // Asumiendo que mapboxContainer está en un Canvas
                                lineRenderer.SetPositions(screenPoints.ToArray());
                                lineRenderer.startColor = trafficColor;
                                lineRenderer.endColor = trafficColor;

                                // Almacenar las posiciones originales
                                Vector3[] initialPointsCopy = new Vector3[screenPoints.Count];
                                screenPoints.CopyTo(initialPointsCopy, 0); // screenPoints son las coordenadas locales ya convertidas
                                originalPositions[lineRenderer] = initialPointsCopy;

                                managedStreetLines.Add(lineRenderer);
                            }
                            else
                            {
                                Debug.LogError("[HereTrafficAPI] El prefab streetLinePrefab no tiene un componente LineRenderer. Destruyendo objeto creado.");
                                Destroy(newLineGO);
                            }
                        }
                        else // Es una actualización, solo cambiar colores
                        {
                            if (currentSegmentIndex < managedStreetLines.Count)
                            {
                                LineRenderer lineToUpdate = managedStreetLines[currentSegmentIndex];
                                if (lineToUpdate != null)
                                {
                                    lineToUpdate.startColor = trafficColor;
                                    lineToUpdate.endColor = trafficColor;
                                }
                                else
                                {
                                    Debug.LogError($"[HereTrafficAPI] Error: Se intentó actualizar una línea nula en el índice {currentSegmentIndex}.");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"[HereTrafficAPI] Desajuste en el número de segmentos durante la actualización. " +
                                                 $"Se recibieron más segmentos ({currentSegmentIndex + 1}) de los dibujados inicialmente ({managedStreetLines.Count}). " +
                                                 "Ignorando segmentos extra. Considere si la geometría de la carretera puede cambiar dinámicamente.");
                            }
                        }
                        currentSegmentIndex++;
                    }

                    


                    if (isInitialSetup)
                    {
                        isInitialLoadSuccessful = true; // Marcar como exitosa
                        Debug.Log($"[HereTrafficAPI] Carga inicial completada. {managedStreetLines.Count} segmentos de tráfico dibujados. {jamFactor}");
                        
                        positionLineRender.Add(managedStreetLines.Count);
                    }
                    else
                    {
                        Debug.Log($"[HereTrafficAPI] Actualización de tráfico completada. {currentSegmentIndex} segmentos procesados/actualizados.{jamFactor}");
                    }
                    
                    if(jamFactor > 4 /*&& ICA.currentAverageICA > 50.0f*/)
                    {
                        LineRenderer lineRenderInitial = managedStreetLines[positionLineRender[currentLinerenderIndex]];
                        LineRenderer lineRenderFinal = managedStreetLines[positionLineRender[currentLinerenderIndex+1]-1];

                        Vector3 positionInitial = lineRenderInitial.GetPosition(0);
                        Vector3 positionFinal = lineRenderFinal.GetPosition(1);

                        // Calcular el punto medio
                        Vector3 midpoint = (positionFinal + positionInitial) * 0.5f;
                        Debug.Log($"Foco de polucion generado en {midpoint}, en el segmento {item["location"]["description"]} coordenadas en pixeles {positionInitial} - {positionFinal}");

                        // Instanciar el prefab en el punto medio
                        GameObject popUp = Instantiate(alertPrefab, popUpsContainer);
                        FocoPolucion scriptPopUp = popUp.GetComponent<FocoPolucion>();
                        scriptPopUp.direccion(item["location"]["description"]);

                        RectTransform popUpRectTransform = popUp.GetComponent<RectTransform>();
                        popUpRectTransform.anchoredPosition = midpoint;
                        

                    }

                    
                    currentLinerenderIndex++;
                }

                // Después de procesar todos los datos nuevos, si es una actualización y hay menos segmentos que antes:
                if (!isInitialSetup && currentSegmentIndex < managedStreetLines.Count)
                {
                    Debug.LogWarning($"[HereTrafficAPI] La actualización de tráfico contiene menos segmentos ({currentSegmentIndex}) " +
                                     $"que los dibujados actualmente ({managedStreetLines.Count}). Desactivando los sobrantes.");
                    for (int i = currentSegmentIndex; i < managedStreetLines.Count; i++)
                    {
                        if (managedStreetLines[i] != null)
                        {
                            managedStreetLines[i].gameObject.SetActive(false);
                            // Opcional: Destruir y remover de la lista si no se espera que reaparezcan
                            // Destroy(managedStreetLines[i].gameObject);
                            // managedStreetLines.RemoveAt(i); i--; // Ajustar índice si se remueve
                        }
                    }
                    // Si se destruyen, considerar remover el rango de la lista:
                    // managedStreetLines.RemoveRange(currentSegmentIndex, managedStreetLines.Count - currentSegmentIndex);
                }
            }
            else
            {
                Debug.LogError($"[HereTrafficAPI] Error en la API de HERE: {request.responseCode} - {request.error}\nURL: {url}\nRespuesta: {request.downloadHandler.text}");
                if (isInitialSetup && isInitialLoadAttempted && !isInitialLoadSuccessful)
                {
                    // Si la carga inicial falla, aún así se intenta iniciar el ciclo de actualizaciones
                    // para que pueda recuperarse en la siguiente llamada si el problema es temporal.
                    Debug.LogWarning("[HereTrafficAPI] La carga inicial falló, pero se intentarán actualizaciones periódicas.");
                    //StartPeriodicUpdates();
                }
            }
        }
        if (isInitialSetup)
        {
            //StartPeriodicUpdates(); // Iniciar el ciclo de actualizaciones periódicas
        }

    }


    /// Determina el color de la línea de tráfico basado en el JamFactor y JamTendency.
    /// Interpola suavemente hacia el estado de tráfico siguiente según la tendencia.
    private Color GetColorByJamFactor(float jamFactor, float jamTendency)
    {
        Color baseColor;
        Color targetColorForLerp;

        if (jamFactor < 0.0f) jamFactor = 0.0f; // Asegurar que no sea negativo

        if (jamFactor < 4.0f) // Libre
        {
            baseColor = colorLibre;
            targetColorForLerp = (jamTendency > 0) ? colorModerado : colorLibre;
        }
        else if (jamFactor < 8.0f) // Moderado
        {
            baseColor = colorModerado;
            targetColorForLerp = (jamTendency > 0) ? colorPesado : colorLibre;
        }
        else if (jamFactor < 10.0f) // Pesado
        {
            baseColor = colorPesado;
            targetColorForLerp = (jamTendency > 0) ? colorBloqueado : colorModerado;
        }
        else // Bloqueado (jamFactor >= 10.0f)
        {
            baseColor = colorBloqueado;
            targetColorForLerp = (jamTendency > 0) ? colorBloqueado : colorPesado;
        }

        // Si la tendencia es insignificante, devolver el color base.
        // Usar un umbral pequeño para evitar Lerp innecesarios.
        if (Mathf.Approximately(jamTendency, 0f) || Mathf.Abs(jamTendency) < 0.01f)
        {
            return baseColor;
        }

        // Interpolar. Mathf.Abs(jamTendency) clampeado a [0,1] para el factor de Lerp.
        return Color.Lerp(baseColor, targetColorForLerp, Mathf.Clamp01(Mathf.Abs(jamTendency)));
    }

    void OnDestroy()
    {
        // Detener todas las corrutinas cuando el objeto se destruye para evitar errores.
        StopAllCoroutines();
        Debug.Log("[HereTrafficAPI] Objeto destruido, todas las corrutinas detenidas.");
    }

    
}
