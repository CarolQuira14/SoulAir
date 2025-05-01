using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CoordinateConverter coordinateConverter;

    public List<HeatZoneController> particleList = new List<HeatZoneController>();

    void Start()
    {

    }

    void Update()
    {
        if (coordinateConverter.lastMapSizeNumber != coordinateConverter.sizeMap)
        {
            actualizarPositionParticles();

            // Actualizar el valor de referencia
            coordinateConverter.lastMapSizeNumber = coordinateConverter.sizeMap;
            coordinateConverter.validacionLimites();
        }
    }

    public void actualizarPositionParticles()
    {
        //Debug.Log("Cambio de tamaño o movimiento detectado. Actualizando partículas...");

        // Actualizar las posiciones
        foreach (HeatZoneController particle in particleList)
        {
            if (particle != null)
            {
                particle.UpdateSensorPosition();
                foreach (HeatZoneController.AQICategory category in particle.categories)
                {
                    category.zoneRadius = 3;
                    category.zoneRadius += coordinateConverter.sizeMap - 1;
                }
            }
        }
    }
}
