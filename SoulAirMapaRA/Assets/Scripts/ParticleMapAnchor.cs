using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ParticleMapAnchor : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Posición relativa en el mapa (0-1)")]
    public Vector2 relativePosition = new Vector2(0.5f, 0.5f); // Centro por defecto

    private RectTransform mapRect;
    private Vector2 lastMapSize;
    private Vector3 initialLocalPos;

    void Start()
    {
        // Obtener referencia al RectTransform del mapa (padre)
        mapRect = transform.parent.GetComponent<RectTransform>();

        if (mapRect == null)
        {
            Debug.LogError("Las partículas deben ser hijas de un objeto con RectTransform");
            return;
        }

        // Guardar posición local inicial
        initialLocalPos = transform.localPosition;

        // Configurar sistema de partículas
        var ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        lastMapSize = mapRect.rect.size;
        Debug.Log($"Tamaño actual del mapa: {mapRect.rect.size}");
    }

    void Update()
    {
        if (mapRect.rect.size != lastMapSize)
        {
            UpdateParticlePosition();
            lastMapSize = mapRect.rect.size;
        }
    }

    void UpdateParticlePosition()
    {
        // Calcular nueva posición manteniendo la relación
        Vector2 newLocalPos = new Vector2(
            (relativePosition.x - 0.5f) * mapRect.rect.width,
            (relativePosition.y - 0.5f) * mapRect.rect.height
        );

        // Mantener posición Z original
        newLocalPos = new Vector3(newLocalPos.x, newLocalPos.y, initialLocalPos.z);

        // Aplicar posición sin tocar la escala
        transform.localPosition = newLocalPos;
        transform.localScale = Vector3.one; // Escala siempre normal
    }
}
