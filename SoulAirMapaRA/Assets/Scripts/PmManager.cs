using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class PmManager : MonoBehaviour
{
    public GameObject panelInfo;
    public GameObject objPrefab;
    private GameObject spawnedObject;
    private Pose pose;
    [SerializeField] private ARRaycastManager aRRaycastManager;
    private bool poseEsValida = false;

    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Se usa para actualizar la pose y ponerla en el plano 
    void Update()
    {
        ActualizarPose();

        if (spawnedObject == null && poseEsValida)
        {
            Debug.Log("Superficie detectada, colocando objeto automáticamente.");
            PonerObjetoEnPlano();
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

    public void AbrirInfoPM(){
        panelInfo.SetActive(true);
    }
    public void CerrarInfoPM(){
        panelInfo.SetActive(false);
    }

    /* public GameObject particlePrefab;
     public int particleCount = 20;
     void Start()
     {
         for (int i = 0; i < particleCount; i++){
             Vector3 randomPosition = Camera.main.transform.position + Random.onUnitSphere * Random.Range(0.3f, 1.0f);
             Instantiate(particlePrefab, randomPosition, Quaternion.identity);
         }
     }

     // Update is called once per frame
     void Update()
     {

     }*/
}
