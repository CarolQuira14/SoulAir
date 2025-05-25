using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Lean.Touch;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PmManager : MonoBehaviour
{
    public GameObject objPrefab;
    public int particleCount = 1;
    private GameObject spawnedObject;
    private Renderer materialPM;
    private Pose pose;
    [SerializeField] private ARRaycastManager aRRaycastManager;
    private bool poseEsValida = false;
    [SerializeField] private int ICAActual;
    private string icaColorTag;

    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        materialPM = objPrefab.GetComponentInChildren<Renderer>();
    }
    // Se usa para actualizar la pose y ponerla en el plano 
    void Update()
    {
        ICAActual = fireStorageListener.icaPC;
        CambioPorICA();

        for (int i = 0; i < particleCount; i++)
        {
            ActualizarPose();
            if (spawnedObject == null && poseEsValida)
            {
                Debug.Log("Superficie detectada, colocando objeto automáticamente.");
                PonerObjetoEnPlano();
            }
        }

    }

    void ActualizarPose()
    {
        var pantallaCentro = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(pantallaCentro, hits, TrackableType.Planes);

        poseEsValida = hits.Count > 0;

        if (poseEsValida)
        {
            pose = hits[0].pose;
        }

    }
    void PonerObjetoEnPlano()
    {
        spawnedObject = Instantiate(objPrefab, pose.position, pose.rotation);
    }

    void CambioPorICA()
    {
        if (ICAActual >= 0 && ICAActual <= 50) // Ica bueno color verde
        {
            icaColorTag = "verde";
            materialPM.sharedMaterial.SetColor("_BaseColor", Color.green);
            particleCount = 10;
        }
        else if (ICAActual >= 51 && ICAActual <= 100) // Ica moderada color amarillo
        {
            icaColorTag = "amarillo";
            materialPM.sharedMaterial.SetColor("_BaseColor", Color.yellow);
            particleCount = 20;
        }
        else if (ICAActual >= 101 && ICAActual <= 150) // Ica dañino para grupos sensibles color naranja
        {
            icaColorTag = "naranja";
            materialPM.sharedMaterial.SetColor("_BaseColor", new Color(1f, 0.5f, 0f));
            particleCount = 30;
        }
        else if (ICAActual >= 151 && ICAActual <= 200) // Ica dañino para grupos sensibles color rojo
        {
            icaColorTag = "rojo";
            materialPM.sharedMaterial.SetColor("_BaseColor", Color.red);
            particleCount = 35;
        }
        else if (ICAActual >= 201 && ICAActual <= 300) // Ica muy dañino color morado
        {
            icaColorTag = "morado";
            materialPM.sharedMaterial.SetColor("_BaseColor", new Color(0.7f, 0f, 1f));
            particleCount = 45;
        }
    }
}
