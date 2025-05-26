using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public CoordinateConverter coordinateConverter;
    public HereTrafficAPI segmentosTrafico;
    public List<HeatZoneController> particleList = new List<HeatZoneController>();
    public GameObject[] focos;
    public GameObject avatar;

    //Funcion que detecta cuando el mapa ha cambiado de tamaño 
    void Update()
    {
        if (coordinateConverter.lastMapSizeNumber != coordinateConverter.sizeMap)
        {
            actualizarPositionParticles();
            coordinateConverter.lastMapSizeNumber = coordinateConverter.sizeMap;
            coordinateConverter.validacionLimites();
            segmentosTrafico.UpdatePosition(coordinateConverter.sizeMap);
            
            focos = (GameObject.FindGameObjectsWithTag("Foco"));
            avatar = GameObject.FindGameObjectWithTag("Avatar");
            actualizarPositionFocos();
            UpdateSize(coordinateConverter.sizeMap);
        }
    }

    //Funcion que actuliza todas las particulas de tamaño en relacion con el mapa 
    public void actualizarPositionParticles()
    {
        foreach (HeatZoneController particle in particleList)
        {
            if (particle != null)
            {
                particle.UpdateSensorPosition();
                //Foreach que actuliza el radio de las particulas
                foreach (HeatZoneController.AQICategory category in particle.categories)
                {
                    category.zoneRadius = 3;
                    category.zoneRadius += coordinateConverter.sizeMap - 1;
                }
            }
        }
    }

    public void actualizarPositionFocos()
    {
        foreach (var item in focos)
        {
            RectTransform transform = item.GetComponent<RectTransform>();
            Vector2 positionVectorFoco = new Vector2(item.GetComponent<FocoPolucion>().posicionesOriginalFoco.x, item.GetComponent<FocoPolucion>().posicionesOriginalFoco.y);
            Vector2 positionNew = coordinateConverter.ConvertUnscaledPixelsToCurrentPixels(positionVectorFoco);
            transform.anchoredPosition = new Vector3(positionNew.x, positionNew.y, 0);
        }

        RectTransform avatarTransform = avatar.GetComponent<RectTransform>();
        Vector2 avatarPosition = coordinateConverter.ConvertUnscaledPixelsToCurrentPixels(new Vector2(-76.52081f, 3.35117f)); // Asumiendo que el avatar está en el centro del mapa
        avatarTransform.anchoredPosition = new Vector3(avatarPosition.x, avatarPosition.y, 0);
    }

    public void UpdateSize(float size)
    {

        foreach (var item in focos)
        {
            Transform transform = item.GetComponent<Transform>();
            Vector3 originalSize = new Vector3(1f, 1f, 0);
            Vector3 newSize = originalSize * (size-0.8f);
            transform.localScale = newSize;
            
        }

        RectTransform avatarTransform = avatar.GetComponent<RectTransform>();
        Vector3 originalSizeAvatar = new Vector3(1f, 1f, 0);
        Vector3 newSizeAvatar = originalSizeAvatar * (size - 0.8f);
        avatarTransform.localScale = newSizeAvatar;
    }

    


}
