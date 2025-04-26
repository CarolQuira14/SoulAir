using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ParticleMapAnchor : MonoBehaviour
{
    [Header("Configuraci�n")]
    [Tooltip("Posici�n relativa en el mapa (0-1)")]
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
            Debug.LogError("Las part�culas deben ser hijas de un objeto con RectTransform");
            return;
        }

        // Guardar posici�n local inicial
        initialLocalPos = transform.localPosition;

        // Configurar sistema de part�culas
        var ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        lastMapSize = mapRect.rect.size;
        Debug.Log($"Tama�o actual del mapa: {mapRect.rect.size}");
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
        // Calcular nueva posici�n manteniendo la relaci�n
        Vector2 newLocalPos = new Vector2(
            (relativePosition.x - 0.5f) * mapRect.rect.width,
            (relativePosition.y - 0.5f) * mapRect.rect.height
        );

        // Mantener posici�n Z original
        newLocalPos = new Vector3(newLocalPos.x, newLocalPos.y, initialLocalPos.z);

        // Aplicar posici�n sin tocar la escala
        transform.localPosition = newLocalPos;
        transform.localScale = Vector3.one; // Escala siempre normal
    }
}
