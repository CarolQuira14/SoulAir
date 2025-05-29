using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mapaHD : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
   
    private Color originalColor; // Guarda el color original del botón
    private Image buttonImage; // Referencia al componente Image del botón
    public GameObject mapa; // Referencia al GameObject del mapa
    private GameObject avatar; // Referencia al GameObject del mapa
    public GameObject mapaViejo; // Referencia al GameObject del mapa
    public GameObject[] focos;


    void Start()
    {
        // Obtén el componente Image del botón
        buttonImage = GetComponent<Image>();
        // Guarda el color original del botón
        originalColor = buttonImage.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Cambia el color a rojo
        buttonImage.color = Color.black;
        mapa.SetActive(true);
        
        avatar = GameObject.FindGameObjectWithTag("Avatar");
        focos = (GameObject.FindGameObjectsWithTag("Foco"));
        foreach (var item in focos)
        {
            Transform transform = item.GetComponent<Transform>();
            if (item != null)
            {
                transform.SetParent(mapa.transform);

            }
        }
        focos = new GameObject[0];
        Transform avatarTransform = avatar.transform;
        if (avatar != null)
        {
            avatarTransform.SetParent(mapa.transform);

        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        avatar = GameObject.FindGameObjectWithTag("Avatar");
        focos = (GameObject.FindGameObjectsWithTag("Foco"));
        Transform transformDelPadreActual = mapaViejo.transform;
        Transform avatarTransform = avatar.transform;
        foreach (var item in focos)
        {
            Transform transform = item.GetComponent<Transform>();
            if (item != null)
            {
                transform.SetParent(transformDelPadreActual);

            }
        }
        focos = new GameObject[0];
        // Restaura el color original
        buttonImage.color = originalColor;
        
        mapa.SetActive(false);
        
        if (avatar != null)
        {
            avatarTransform.SetParent(transformDelPadreActual);

        }

    }
}
