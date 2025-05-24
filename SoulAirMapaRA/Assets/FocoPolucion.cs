using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FocoPolucion : MonoBehaviour
{

    [Header("Referencias")]
    
    [SerializeField] private Button childButton; // Referencia al Button en el hijo
    [SerializeField] private GameObject prefabToInstantiate; // Referencia al prefab a instanciar
    [SerializeField] private Animator objetoAnimator; // Referencia al componente Animator

    public Transform parentTransformMapbox;
    private string direccionFoco  = "hola";
    private GameObject panel;
    private PopUpControl popUpScript;

    public Vector2 posicionesOriginalFoco;



    void Awake()
    {   
        if (childButton == null)
        {
            childButton = GetComponent<Button>();
        }

        if (panel == null)
        {
            panel = GameObject.FindGameObjectWithTag("PopUpInfo");
        }
    }
    void Start()
    {
        posicionesOriginalFoco = GetComponent<RectTransform>().anchoredPosition;
        childButton.onClick.AddListener(OnButtonClick);
        objetoAnimator = GetComponent<Animator>();
        StartCoroutine(animacion());
        panel.SetActive(false);
        
    }

    private void Update()
    {
        // Añadir verificación de null para panel y popUpScript
        if (panel != null && !panel.activeSelf && popUpScript != null)
        {
            popUpScript.HidePopUp();
        }
    }

    

    private IEnumerator animacion()
    {
        // Esperar 2 segundos antes de activar la animación
        yield return new WaitForSeconds(1f);

        // Activar el parámetro "show" en el Animator si la referencia no es nula
        if (objetoAnimator != null)
        {
            objetoAnimator.SetBool("show", true);
        }
    }

    public void direccion(string apiDireccion) 
    {
         direccionFoco = apiDireccion;
    }


    // Esta función se llamará cuando se haga clic en el botón hijo
    private void OnButtonClick()
    {
        Debug.Log("Botón hijo clickeado. Intentando instanciar prefab.");
        parentTransformMapbox = GetComponentInParent<Transform>();
        // 3. Instanciar el otro prefab
        if (prefabToInstantiate != null)
        {
            panel.SetActive(true);
            GameObject instantiatedObject = Instantiate(prefabToInstantiate);
            // Asegúrate de que el panel.transform no sea null antes de usarlo como padre
            if (panel.transform != null)
            {
                instantiatedObject.transform.SetParent(panel.transform, false);
            }

            popUpScript = instantiatedObject.GetComponent<PopUpControl>();

            if (popUpScript != null)
            {
                popUpScript.ShowPopUp(direccionFoco);
            }

        }

    }
}
