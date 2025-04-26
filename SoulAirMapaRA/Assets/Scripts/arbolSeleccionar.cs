using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class arbolSeleccionar : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] selectableObjects; // Objetos que se pueden seleccionar
    private GameObject selectedObject; // El objeto actualmente seleccionado
    public Camera arCamera; // Cámara de realidad aumentada
    public ARRaycastManager arRaycastManager; // AR Raycast Manager para detectar planos
    public ARPlaneManager arPlaneManager; // Plane manager para los planos
    public Button[] objectButtons; // Botones en la UI para seleccionar los objetos
    

    static List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Lista de raycast hits

    void Start()
    {
        // Asignar los botones de la UI para seleccionar objetos
        for (int i = 0; i < objectButtons.Length; i++)
        {
            int index = i; // Necesario para evitar el problema de "closure" en bucles
            objectButtons[i].onClick.AddListener(() => SelectObject(index));
        }
    }

    void Update()
    {
        // Detectar si se ha tocado la pantalla y si no está sobre la UI
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUI())
        {
            Touch touch = Input.GetTouch(0);
            if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
            {
                Pose hitPose = hits[0].pose;

                if (selectedObject != null)
                {
                    // Instanciar el objeto seleccionado en la posición del hit
                    Instantiate(selectedObject, hitPose.position, hitPose.rotation);
                }
            }
        }
    }

    // Función para que el usuario seleccione un objeto desde la UI
    public void SelectObject(int objectIndex)
    {
        selectedObject = selectableObjects[objectIndex]; // Selecciona el objeto por índice
    }

    // Verifica si el toque ocurrió sobre la UI
    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        return false;
    }
}
