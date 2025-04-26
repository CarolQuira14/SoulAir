using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InicializarObjetos : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ARPlaneManager arPlaneManager;
    //[SerializeField] private GameObject factoryPrefab;
    [SerializeField] private GameObject particlePrefab;
    //[SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject panelConfirm;
   
    public bool objectsSpawned=false;
    //public int numberOfObjects = 5;
    public float ICA;
    public int particleCount;

    
    private List<ARPlane> planes = new List<ARPlane>();   // Lista de planos detectados
    private void Start()
    {
        ICA = Random.Range(26,85);
        particleCount=Mathf.RoundToInt(ICA/2);
       
    }
    // Referencia al ARPlaneManager, que se encarga de detectar planos
    private void OnEnable()
    {
        arPlaneManager.planesChanged += PlanesFound;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= PlanesFound;
    }

    // Este m�todo se llama cuando ARCore detecta nuevos planos
    private void PlanesFound(ARPlanesChangedEventArgs planeData)
    {
        if (planeData.added != null && planeData.added.Count > 0)
        {
            planes.AddRange(planeData.added);
        }

        if (!objectsSpawned)  // Solo instanciar objetos si no se han instanciado antes
        {
            foreach (var plane in planes)
            {
                if (plane.extents.x * plane.extents.y >= 0.5f)
                {
                    Vector3 randomPositionOnPlane = GetRandomPositionOnPlane(plane);

                    // Instanciar el prefab de la particula en la posici�n aleatoria
                    SpawnParticlesAroundCamera();

                    objectsSpawned = true;
                    //StopPlaneDetection(); // Detener la detecci�n de planos una vez instanciado
                    panelConfirm.SetActive(true);
                    break;  // Detener el loop despu�s de instanciar el objeto
                }
            }
        }
    }

    // Genera una posici�n aleatoria en el plano dado
    private Vector3 GetRandomPositionOnPlane(ARPlane plane)
    {
        Vector2 randomInPlane = UnityEngine.Random.insideUnitCircle * plane.extents.x; // Usa el radio del plano
        return new Vector3(randomInPlane.x + plane.center.x, plane.center.y, randomInPlane.y + plane.center.z);
    }

    public void StopPlaneDetection()
    {
        arPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;

        foreach (var plane in planes)
        {
            plane.gameObject.SetActive(false);
        }
        
    }
    /*void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Solo proceder si se han a�adido nuevos planos
        if (args.added.Count > 0 && !objectsSpawned)
        {
            foreach (var plane in args.added)
            {
                detectedPlanes.Add(plane);
            }

            for (int i = 0; i < numberOfObjects; i++)
            {
                ARPlane randomPlane = detectedPlanes[Random.Range(0, detectedPlanes.Count)];
                Vector3 randomPositionOnPlane = GetRandomPositionOnPlane(randomPlane);
                Instantiate(factoryPrefab, randomPositionOnPlane, Quaternion.identity);
            }

            objectsSpawned = true;
            detectedPlanes.Clear();
        }
    }*/

    /*   void Update()
       {
           // Si hay planos detectados, instanciar los objetos en ellos
           if (detectedPlanes.Count > 0)
           {
               for (int i = 0; i < numberOfObjects; i++)
               {
                   // Seleccionar un plano aleatorio
                   ARPlane randomPlane = detectedPlanes[Random.Range(0, detectedPlanes.Count)];

                   // Generar un punto aleatorio en el plano
                   Vector3 randomPositionOnPlane = GetRandomPositionOnPlane(randomPlane);

                   // Instanciar el prefab en la posici�n aleatoria sobre el plano
                   Instantiate(factoryPrefab, randomPositionOnPlane, Quaternion.identity);
               }

               // Vaciar la lista despu�s de instanciar objetos para evitar sobrecarga
               detectedPlanes.Clear();
           }
       }*/

    Vector3 GetRandomPositionOnPlanexxx(ARPlane plane)
    {
        // Obtener el tama�o del plano (extents) en el eje X y Z
        Vector3 planeCenter = plane.transform.position;
        Vector2 planeSize = plane.size / 2;  // Dividir por 2 para trabajar con el centro

        // Generar una posici�n aleatoria dentro del plano
        float randomX = Random.Range(-planeSize.x, planeSize.x);
        float randomZ = Random.Range(-planeSize.y, planeSize.y);

        // Devolver una posici�n en el plano
        return new Vector3(planeCenter.x + randomX, planeCenter.y, planeCenter.z + randomZ);
    }

    private void SpawnParticlesAroundCamera(){
        Transform cameraTransform = Camera.main.transform;
        float minDistance = 0.5f; // Distancia mínima desde la cámara
        float maxDistance = 1.5f; // Distancia máxima desde la cámara
        for (int i = 0; i < particleCount; i++){
            Vector3 randomDirection = Random.onUnitSphere; // Dirección aleatoria
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 spawnPosition = cameraTransform.position + randomDirection * distance;

            Instantiate(particlePrefab, spawnPosition, Quaternion.identity);
        }

    }
}
