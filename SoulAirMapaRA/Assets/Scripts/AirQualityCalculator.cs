using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Image = UnityEngine.UI.Image;

public class AirQualityCalculator : MonoBehaviour
{
    [Header("Avatar")]
    public Vector2 avatarCoords = new Vector2(-76.52081f, 3.35117f);
    [Tooltip("Contenedor donde se instanciarán el avatar")]
    public Transform mapboxContainer;
    [SerializeField] private List<GameObject> avatars = new List<GameObject>();

    [Header("Referencias")]
    public CoordinateConverter coordinateConverter;

    [Header("Configuración")]
    public float exposureRadius = 0.012f; // Radio en grados geográficos
    public TMP_Text updateInterval; // Cada cuánto se actualiza el cálculo
    public int icaValuePC;
    public int icaValueUV;

    [Header("Referencias")]
    public List<HeatZoneController> airQualityStations = new List<HeatZoneController>();

    [Header("Referencias UI")]
    public Image radialIndicator; // Imagen con Radial 180 (Bottom)

    public Image footer;
    public Image avatar;
    public Image camara;
    public Image circle;
    public Image gps;
    public Image slider;
    public Image HD;

    public Image avatarBorder;
    public Image camaraBorder;
    public Image circleBorder;
    public Image gpsBorder;
    public Image HDBorder;

    public Color greenColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color yellowColor = new Color(0.8f, 0.8f, 0.2f, 1f);
    public Color orangeColor = new Color(0.8f, 0.5f, 0.2f, 1f);
    public Color redColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    public float darkenFactor = 0.8f; // Valor entre 0 (negro) y 1 (sin cambio)

    public GameObject popUpBueno;
    public GameObject popUpMalo;
    public GameObject popUpMalo02;
    private GameObject panel;
    private PopUpControlEsc_1 mensajeScript;

    public float currentAverageICA;
    private bool x = false;
    public GameObject avatarUI;

    void Start()
    {
        // Encontrar todas las estaciones automáticamente (opcional)
        airQualityStations.AddRange(FindObjectsOfType<HeatZoneController>());
        CalculateAverageICA();
    }

    private Color colorDarkeness(Color color)
    {
        Color darkenedColor = new Color(
        color.r * darkenFactor,
        color.g * darkenFactor,
        color.b * darkenFactor,
        color.a);
        return darkenedColor;
    }

    public IEnumerator InstanciarPopUpAvatar(GameObject mensaje)
    {
        yield return new WaitForSeconds(2f);
        GameObject avatarMensaje = GameObject.FindGameObjectWithTag("Avatar");
        Debug.LogWarning("Mostrando PopUp Avatar: ");

        GameObject instantiatedObject = null; // Track the instantiated object

        if (avatarMensaje != null)
        {
            Debug.LogWarning("Entre 01");
            if (mensaje != null)
            {
                Debug.LogWarning("Entre 02");
                panel = GameObject.FindGameObjectWithTag("PopUpAvatar");
                if (panel.transform != null)
                {
                    Debug.LogWarning("Entre 03");
                    instantiatedObject = Instantiate(mensaje);
                    instantiatedObject.transform.SetParent(panel.transform, false);
                    mensajeScript = instantiatedObject.GetComponent<PopUpControlEsc_1>();

                    if (mensajeScript != null)
                    {
                        Debug.LogWarning("Entre 03");
                        mensajeScript.ShowPopUp();
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No Funciona");
        }

        yield return new WaitForSeconds(5f);
        if (mensajeScript != null)
        {
            mensajeScript.HidePopUp();
        }

        yield return new WaitForSeconds(2f);
        if (instantiatedObject != null)
        {
            Destroy(instantiatedObject); // Destroy the instance, not the prefab
        }
    }



    private GameObject FindGameObjectByExactName(string objectName)
    {
        // Validar que el nombre no esté vacío o nulo
        if (string.IsNullOrEmpty(objectName))
        {
            Debug.LogWarning("El nombre proporcionado está vacío o es nulo");
            return null;
        }

        // Iterar a través de la lista de GameObjects
        for (int i = 0; i < avatars.Count; i++)
        {
            // Verificar que el GameObject no sea nulo
            if (avatars[i] != null)
            {
                // Comparar el nombre del GameObject con el nombre buscado
                if (avatars[i].name.Equals(objectName))
                {
                    Debug.Log($"GameObject '{objectName}' encontrado en el índice {i}");
                    return avatars[i];
                }
            }
        }

        // Si llegamos aquí, el GameObject no fue encontrado
        Debug.LogWarning($"GameObject con nombre '{objectName}' no encontrado en la lista");
        return null;
    }

    public void FindAndInstantiate(string objectName)
    {
        // Paso 1: Buscar el GameObject en la lista
        GameObject prefabToInstantiate = FindGameObjectByExactName(objectName);
        // Paso 2: Verificar si se encontró el GameObject
        if (prefabToInstantiate == null)
        {
            Debug.LogError($"No se puede instanciar '{objectName}' porque no se encontró en la lista");
        }
        // Paso 3: Instanciar el GameObject
        GameObject instantiatedObject;

        try
        {
            // Instanciar el objeto
            instantiatedObject = Instantiate(prefabToInstantiate, mapboxContainer);
            RectTransform avatarTransform = instantiatedObject.GetComponent<RectTransform>();
            Vector2 localPosAvatar = coordinateConverter.ConvertCoordinatestoPixels(avatarCoords);
            avatarTransform.anchoredPosition += localPosAvatar;
            Debug.Log($"GameObject '{objectName}' instanciado exitosamente en posición {localPosAvatar}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al instanciar '{objectName}': {e.Message}");
        }
    }

    public void RemoveAvatar()
    {
        // Buscar el GameObject en la lista
        GameObject avatarToRemove = GameObject.FindGameObjectWithTag("Avatar"); 
        // Verificar si se encontró el GameObject
        if (avatarToRemove != null)
        {
            // Destruir el GameObject
            Destroy(avatarToRemove);
            Debug.Log("Avartar eliminado exitosamente");
        }
        else
        {
            Debug.LogWarning("No se pudo eliminar el avatar porque no se encontró");
        }
    }

    private void Update()
    {
        if (fireStorageListener.cambioICA)
        {
            CalculateAverageICA();
        }
    }
    /*Corregir la API que devuelva las coordinadas de los sensores para asi saber cual sensor del la app actulizar el ICA*/
    //Actualiza el promedio cuando hay una actulizacion de los datos del ica de alguno de los sensores
    public void CalculateAverageICA() 
    {
        //Obtiene los valores del ICA de cada sensor
        icaValuePC = fireStorageListener.icaPC;
        icaValueUV = fireStorageListener.icaUV;

        //Nueva lista de valores y estaciones que se actulizaron el ICA 
        //List<HeatZoneController> stationsInRange = new List<HeatZoneController>();
        List<int> icaValues = new List<int>();


        foreach (var station in airQualityStations)
        {
            if (station == null) continue;

            //Actualizacion del ica de cada sensor se usa la posicion exacta para poder identificar cual es (CORREGIR)
            if (station.sensorCoords == new Vector2(-76.543f, 3.336f)) { station.currentICA = icaValuePC; }
            else { station.currentICA = icaValueUV; }

            // Calcular distancia geográfica entre el avatar y la estación
            float distance = CalculateGeographicDistance(
                avatarCoords,
                station.sensorCoords
            );

            //Agrega el valor del ICA si esta en el area de exposicion
            if (distance <= exposureRadius)
            {
                //stationsInRange.Add(station);
                icaValues.Add(station.currentICA);
            }
        }


        fireStorageListener.cambioICA = false;
        // Metodo para calcular el promedio
        if (icaValues.Count > 0)
        {
            currentAverageICA = 0;
            foreach (int ICA in icaValues)
            {
                currentAverageICA += ICA;
            }
            currentAverageICA = currentAverageICA / icaValues.Count;
            updateInterval.text = currentAverageICA.ToString();
            RemoveAvatar();
            UpdateRadialIndicator();
            Debug.Log($"ICA Promedio: {currentAverageICA} (Estaciones: {icaValues.Count})");
        }
        else
        {
            currentAverageICA = 0;
            Debug.Log("No hay estaciones en el rango de exposición");
        }
    }


    // Calcula distancia entre dos puntos geográficos (simplificado)
    float CalculateGeographicDistance(Vector2 coord1, Vector2 coord2)
    {
        // Distancia euclidiana simplificada para áreas pequeñas
        return Mathf.Sqrt(
            Mathf.Pow(coord1.x - coord2.x, 2) +
            Mathf.Pow(coord1.y - coord2.y, 2)
        );
    }


    // Funcion que actualiza la posición del avatar para calcular el area de exposicion
    public void UpdateAvatarPosition(Vector2 newCoords)
    {
        avatarCoords = newCoords;
        CalculateAverageICA(); // Recalcular inmediatamente
        Debug.Log("este es el valor de currentAvarageICa " + currentAverageICA);
    }

    //Funcion que actuliza los colores del UI
    void UpdateRadialIndicator()
    {
        if (radialIndicator == null) return;
        radialIndicator.fillAmount = Mathf.Clamp01(currentAverageICA / 200f);
        switch (currentAverageICA)
        {
            case <= 50:
                radialIndicator.color = greenColor;
                circle.color = greenColor;
                footer.color = greenColor;
                avatar.color = greenColor;
                camara.color = greenColor;
                gps.color = greenColor;
                slider.color = greenColor;
                HD.color = greenColor;
                avatarBorder.color = colorDarkeness(greenColor);
                camaraBorder.color = colorDarkeness(greenColor);
                circleBorder.color = colorDarkeness(greenColor);
                gpsBorder.color = colorDarkeness(greenColor);
                HDBorder.color = colorDarkeness(greenColor);
                avatarUI.SetActive(false);
                FindAndInstantiate("avatarInicial");
                if (x)
                {
                    StartCoroutine(InstanciarPopUpAvatar(popUpBueno));
                }
                x = true;
                break;
            case <= 100:
                radialIndicator.color = yellowColor;
                circle.color = yellowColor;
                footer.color = yellowColor;
                avatar.color = yellowColor;
                camara.color = yellowColor;
                gps.color = yellowColor;
                slider.color = yellowColor;
                HD.color = yellowColor;
                avatarBorder.color = colorDarkeness(yellowColor);
                camaraBorder.color = colorDarkeness(yellowColor);
                circleBorder.color = colorDarkeness(yellowColor);
                gpsBorder.color = colorDarkeness(yellowColor);
                HDBorder.color = colorDarkeness(yellowColor);
                avatarUI.SetActive(true);
                FindAndInstantiate("avatarAmarillo");
                StartCoroutine(InstanciarPopUpAvatar(popUpMalo));
                StartCoroutine(InstanciarPopUpAvatar(popUpMalo02));

                break;
            case <= 150:
                radialIndicator.color = orangeColor;
                circle.color = orangeColor;
                footer.color = orangeColor;
                avatar.color = orangeColor;
                camara.color = orangeColor;
                gps.color = orangeColor;
                slider.color = orangeColor;
                HD.color = orangeColor;
                avatarBorder.color = colorDarkeness(orangeColor);
                camaraBorder.color = colorDarkeness(orangeColor);
                circleBorder.color = colorDarkeness(orangeColor);
                gpsBorder.color = colorDarkeness(orangeColor);
                HDBorder.color = colorDarkeness(orangeColor);
                FindAndInstantiate("avatarNaranja");
                
                break;
            default:
                radialIndicator.color = redColor;
                circle.color = redColor;
                footer.color = redColor;
                avatar.color = redColor;
                camara.color = redColor;
                gps.color = redColor;
                slider.color = redColor;
                HD.color = redColor;
                avatarBorder.color = colorDarkeness(redColor);
                camaraBorder.color = colorDarkeness(redColor);
                circleBorder.color = colorDarkeness(redColor);
                gpsBorder.color = colorDarkeness(redColor);
                HDBorder.color = colorDarkeness(redColor);
                FindAndInstantiate("avatarRojo");
                break;
        }
        radialIndicator.CrossFadeColor(radialIndicator.color, 0.5f, true, true);
    }



}