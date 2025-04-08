using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class AirQualityCalculator : MonoBehaviour
{
    [Header("Configuración")]
    public Vector2 avatarCoords = new Vector2(-76.52081f, 3.35117f);
    public float exposureRadius = 0.012f; // Radio en grados geográficos
    public TMP_Text updateInterval; // Cada cuánto se actualiza el cálculo

    [Header("Referencias")]
    public List<HeatZoneController> airQualityStations = new List<HeatZoneController>();

    [Header("Referencias UI")]
    public Image radialIndicator; // Imagen con Radial 180 (Bottom)
    public Image circle;
    public Image footer;
    public Color greenColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color yellowColor = new Color(0.8f, 0.8f, 0.2f, 1f);
    public Color orangeColor = new Color(0.8f, 0.5f, 0.2f, 1f);
    public Color redColor = new Color(0.8f, 0.2f, 0.2f, 1f);


    private float currentAverageICA;

    void Start()
    {
        // Encontrar todas las estaciones automáticamente (opcional)
        airQualityStations.AddRange(FindObjectsOfType<HeatZoneController>());
        CalculateAverageICA();
        UpdateRadialIndicator();
    }



    void CalculateAverageICA() //Actualiza el promedio cuando hay una actulizacion de los datos del ica de alguno de los sensores
    {
        List<HeatZoneController> stationsInRange = new List<HeatZoneController>();
        List<int> icaValues = new List<int>();

        foreach (var station in airQualityStations)
        {
            if (station == null) continue;


            
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

        // Calcular promedio si hay estaciones en el rango
        if (icaValues.Count > 0)
        {
            foreach (int ICA in icaValues)
            {
                currentAverageICA += ICA;
            }
            currentAverageICA = currentAverageICA / icaValues.Count;
            updateInterval.text = currentAverageICA.ToString();
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
    }

    void UpdateRadialIndicator()
    {
        if (radialIndicator == null) return;

        // Actualizar fill amount (0-1 basado en 0-200)
        radialIndicator.fillAmount = Mathf.Clamp01(currentAverageICA / 200f);

        // Cambiar color según rango ICA
        if (currentAverageICA <= 50)
        {
            radialIndicator.color = greenColor;
            circle.color = greenColor;
            footer.color = greenColor;
        }
        else if (currentAverageICA <= 100)
        {
            radialIndicator.color = yellowColor;
            circle.color = yellowColor;
            footer.color = yellowColor;
        }
        else if (currentAverageICA <= 150)
        {
            radialIndicator.color = orangeColor;
            circle.color = orangeColor;
            footer.color = orangeColor;
        }
        else
        {
            radialIndicator.color = redColor;
            circle.color = redColor;
            footer.color = redColor;
        }

        // Opcional: Efecto de transición suave
        radialIndicator.CrossFadeColor(radialIndicator.color, 0.5f, true, true);
    }



}