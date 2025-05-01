using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AirQualityCalculator : MonoBehaviour
{
    [Header("Configuración")]
    public Vector2 avatarCoords = new Vector2(-76.52081f, 3.35117f);
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

    public Image avatarBorder;
    public Image camaraBorder;
    public Image circleBorder;
    public Image gpsBorder;

    public Color greenColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color yellowColor = new Color(0.8f, 0.8f, 0.2f, 1f);
    public Color orangeColor = new Color(0.8f, 0.5f, 0.2f, 1f);
    public Color redColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    public float darkenFactor = 0.8f; // Valor entre 0 (negro) y 1 (sin cambio)


    private float currentAverageICA;

    void Start()
    {
        // Encontrar todas las estaciones automáticamente (opcional)
        airQualityStations.AddRange(FindObjectsOfType<HeatZoneController>());
        
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

    private void Update()
    {

        

        if (fireStorageListener.cambioICA)
        {
            CalculateAverageICA();
        }
    }

    public void CalculateAverageICA() //Actualiza el promedio cuando hay una actulizacion de los datos del ica de alguno de los sensores
    {
        icaValuePC = fireStorageListener.icaPC;
        icaValueUV = fireStorageListener.icaUV;

        List<HeatZoneController> stationsInRange = new List<HeatZoneController>();
        List<int> icaValues = new List<int>();

        foreach (var station in airQualityStations)
        {
            if (station == null) continue;

            //Actualizacion del ica de cada sensor se usa la posicion exacta para poder identificar cual es 
            if (station.sensorCoords == new Vector2(-76.5307f, 3.3386f)) { station.currentICA = icaValuePC; }
            else { station.currentICA = icaValueUV; }

            // Calcular distancia geográfica entre el avatar y la estación
            float distance = CalculateGeographicDistance(
                avatarCoords,
                station.sensorCoords
            );


            if (distance <= exposureRadius)
            {

                stationsInRange.Add(station);
                icaValues.Add(station.currentICA);
            }


        }

        
        fireStorageListener.cambioICA = false;

        // Calcular promedio si hay estaciones en el rango
        if (icaValues.Count > 0)
        {
            foreach (int ICA in icaValues)
            {
                currentAverageICA += ICA;
            }
            currentAverageICA = currentAverageICA / icaValues.Count;
            updateInterval.text = currentAverageICA.ToString();
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

    // Método para actualizar posición del avatar
    public void UpdateAvatarPosition(Vector2 newCoords)
    {
        avatarCoords = newCoords;
        CalculateAverageICA(); // Recalcular inmediatamente
        Debug.Log("este es el valor de currentAvarageICa " + currentAverageICA);
    }

    void UpdateRadialIndicator()
    {
        if (radialIndicator == null) return;
        // Actualizar fill amount (0-1 basado en 0-200)
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
                avatarBorder.color = colorDarkeness(greenColor);
                camaraBorder.color = colorDarkeness(greenColor);
                circleBorder.color = colorDarkeness(greenColor);
                gpsBorder.color = colorDarkeness(greenColor);
                break;
            case <= 100:
                radialIndicator.color = yellowColor;
                circle.color = yellowColor;
                footer.color = yellowColor;
                avatar.color = yellowColor;
                camara.color = yellowColor;
                gps.color = yellowColor;
                slider.color = yellowColor;
                avatarBorder.color = colorDarkeness(yellowColor);
                camaraBorder.color = colorDarkeness(yellowColor);
                circleBorder.color = colorDarkeness(yellowColor);
                gpsBorder.color = colorDarkeness(yellowColor);
                break;
            case <= 150:
                radialIndicator.color = orangeColor;
                circle.color = orangeColor;
                footer.color = orangeColor;
                avatar.color = orangeColor;
                camara.color = orangeColor;
                gps.color = orangeColor;
                slider.color = orangeColor;
                avatarBorder.color = colorDarkeness(orangeColor);
                camaraBorder.color = colorDarkeness(orangeColor);
                circleBorder.color = colorDarkeness(orangeColor);
                gpsBorder.color = colorDarkeness(orangeColor);
                break;
            default:
                radialIndicator.color = redColor;
                circle.color = redColor;
                footer.color = redColor;
                avatar.color = redColor;
                camara.color = redColor;
                gps.color = redColor;
                slider.color = redColor;
                avatarBorder.color = colorDarkeness(redColor);
                camaraBorder.color = colorDarkeness(redColor);
                circleBorder.color = colorDarkeness(redColor);
                gpsBorder.color = colorDarkeness(redColor);
                break;
        }
        // Opcional: Efecto de transición suave
        radialIndicator.CrossFadeColor(radialIndicator.color, 0.5f, true, true);
    }



}