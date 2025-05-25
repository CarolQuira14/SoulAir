using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Movement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    [Header("Configuración")]
    public float dragSensitivity = 1f;
    public CoordinateConverter size;

    private RectTransform mapRectTransform;
    public bool isDragging = false;
    private Vector2 originalSize;

    void Awake()
    {
        mapRectTransform = GetComponent<RectTransform>();
        originalSize = mapRectTransform.rect.size;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log($"Funciono {mapRectTransform.rect.size}");
        if (/*Input.touchCount == 1&&*/ mapRectTransform.rect.size != originalSize) // Solo mover con un dedo
        {

            isDragging = true;

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if (!isDragging) return;
        // Bueno para movimiento relativo/arrastre
        mapRectTransform.anchoredPosition += eventData.delta * dragSensitivity;
        Vector2 clampedPosition = mapRectTransform.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -180f * size.sizeMap, 180f * size.sizeMap);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -320f * size.sizeMap, 320f * size.sizeMap);
        mapRectTransform.anchoredPosition = clampedPosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            //Debug.Log("Dejo de Funcionar");
            isDragging = false;
            size.updateZoomMap(true);

        }
        

    }


    // Opcional: Limitar movimiento dentro de un área
    public void ClampMapPosition()
    {
        Vector2 clampedPosition = mapRectTransform.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -360f, 360);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -640f, 720);
        mapRectTransform.anchoredPosition = clampedPosition;
    }
}
