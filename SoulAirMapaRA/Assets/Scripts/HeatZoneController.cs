using UnityEngine;
using System.Collections.Generic;
//using static UnityEditor.Experimental.GraphView.Port;


[RequireComponent(typeof(RectTransform))]
public class HeatZoneController : MonoBehaviour
{
    [System.Serializable]
    public class AQICategory
    {
        public int minICA;
        public int maxICA;
        public Color zoneColor;
        [Range(0, 1)] public float opacity = 1f;
        public float zoneRadius;
        public float colorTransitionSpeed = 2f;

        public Color ColorWithOpacity
        {
            get
            {
                Color c = zoneColor;
                c.a = opacity;
                return c;
            }
        }
    }


    [Header("Configuración ICA")]
    public List<AQICategory> categories = new List<AQICategory>
    {
        new AQICategory { minICA = 0, maxICA = 50, zoneColor = Color.green, zoneRadius = 2f, opacity = 0.2f  },
        new AQICategory { minICA = 51, maxICA = 100, zoneColor = Color.yellow, zoneRadius = 2f, opacity = 0.3f },
        new AQICategory { minICA = 101, maxICA = 150, zoneColor = new Color(1f, 0.5f, 0f), zoneRadius = 3f, opacity = 0.4f },
        new AQICategory { minICA = 151, maxICA = 200, zoneColor = Color.red, zoneRadius = 3f, opacity = 0.5f }
    };

    [Header("Control ICA")]
    [Range(0, 200)] public int currentICA; //CAMBIAR JSON
    private int lastICA;

    [Header("Configuración de Material")]
    public Material particleMaterial;

    [Header("Configuración")]
    public Vector2 sensorCoords; // Coordenadas GPS reales del sensor

    [Header("Referencias")]
    public CoordinateConverter coordinateConverter;

    private ParticleSystem particleSystem;
    private Color targetColor;
    private float targetRadius;


    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();



        UpdateTargetValues();
        lastICA = currentICA;

        // Configurar material para manejar transparencia
        if (particleMaterial != null)
        {
            particleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            particleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            particleMaterial.renderQueue = 3000;
        }

        coordinateConverter.lastMapSizeNumber = coordinateConverter.sizeMap;
        UpdateSensorPosition();
        
    }

    void Update()
    {
        if (currentICA != lastICA)
        {
            UpdateTargetValues();
            lastICA = currentICA;
        }

        


        UpdateParticleProperties();
    }

    public void UpdateSensorPosition()
    {
        // Convertir coordenadas GPS a posición local
        Vector2 localPos = coordinateConverter.ZoomMapPositionParticles(sensorCoords);

        // Aplicar posición al sistema de partículas
        RectTransform particleRect = particleSystem.GetComponent<RectTransform>();
        particleRect.anchoredPosition = localPos;

        Debug.Log($"Sensor en {sensorCoords} posicionado en {localPos}");
    }

    void UpdateTargetValues()
    {
        AQICategory currentCategory = categories.Find(c => currentICA >= c.minICA && currentICA <= c.maxICA);

        if (currentCategory != null)
        {
            targetColor = currentCategory.zoneColor;
            targetRadius = currentCategory.zoneRadius;
        }
    }

    void UpdateParticleProperties()
    {
        var main = particleSystem.main;
        var shape = particleSystem.shape;

        AQICategory currentCategory = categories.Find(c => currentICA >= c.minICA && currentICA <= c.maxICA);

        if (currentCategory == null) return;

        // Transición gradual de color CON opacidad
        Color currentColor = main.startColor.color;
        Color targetColorWithOpacity = currentCategory.ColorWithOpacity;

        Color newColor = Color.Lerp(currentColor, targetColorWithOpacity,
            Time.deltaTime * currentCategory.colorTransitionSpeed);

        // Transición gradual de tamaño
        float currentRadius = shape.radius;
        float newRadius = Mathf.Lerp(currentRadius, currentCategory.zoneRadius,
            Time.deltaTime * currentCategory.colorTransitionSpeed);

        // Aplicar cambios
        main.startColor = newColor;
        shape.radius = newRadius;
    }
}