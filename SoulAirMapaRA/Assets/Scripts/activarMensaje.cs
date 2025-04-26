using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activarMensaje : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panelMensaje1;
    private GameObject modelo3d;
    void Start()
    {
        modelo3d = GameObject.FindGameObjectWithTag("sistemaR");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (modelo3d!=null)
        {
            Debug.Log("Sistema respiratorio detectado");
            panelMensaje1.SetActive(true);
        }
        else
        {
            Debug.Log("Sistema respiratorio aún no detectado");
        }
    }
}
