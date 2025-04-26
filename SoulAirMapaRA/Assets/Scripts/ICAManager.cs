using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ICAManager : MonoBehaviour
{
    GameObject targetObject;
    public int ica = 50;
    public Text ICATxt;
   
    public Image cirucloIca2;
    public Transform userCamera;
    GameObject[] fabricas;
    GameObject[] arboles;
    public float detectionRadius = 5f;
    
    public GameObject advertencia;
    public GameObject advertencia1;
   // int n = 1;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        fabricas = GameObject.FindGameObjectsWithTag("fabrica");
        arboles = GameObject.FindGameObjectsWithTag("arbol");
        
        ReduceICAByArboles();
        ICATxt.text = ica.ToString();
        cambiarIca();
        cambioColor();
       
    }
    public void cambiarIca()
    {
        foreach (GameObject fabrica in fabricas)
        {//sh
            float distance = Vector3.Distance(userCamera.position, fabrica.transform.position);

            if (distance < 0.5f)
            {
                UpdateICA(201, 300);
            }
            else if (distance < 1f)
            {
                UpdateICA(151, 200);
                
            }
            else if (distance < 2f)
            {
                UpdateICA(101, 150);
            }
            else if (distance < 4f)
            {
                UpdateICA(51, 100);
            }
            else if (distance < 6f)
            {
                UpdateICA(10, 50);

            }
            else
            {
                UpdateICA(0, 25);
                
            }
        }
        if(ica>150 && ica <= 200)
        {
            advertencia.SetActive(true);
        }
        else
        {
            advertencia.SetActive(false);
        }

        if(ica>200)
        {
            advertencia1.SetActive(true);
        }
        else
        {
            advertencia1.SetActive(false);
        }
        
    }
    void UpdateICA( int minICA, int maxICA)
    {
        int increaseAmount = Mathf.RoundToInt(Time.deltaTime * 50f);
        ica += increaseAmount;
        ica = Mathf.Clamp(ica, minICA, maxICA);
    }
    void ReduceICAByArboles()
    {
        int treeCount = arboles.Length;
       // Debug.Log("valorArboles: "+treeCount);
        int decreaseAmount = Mathf.RoundToInt(treeCount * 60f * Time.deltaTime);  // Ajustar la reducción por árbol
        ica -= decreaseAmount;
        ica = Mathf.Clamp(ica, 0, 300);  // Asegurarse de que el ICA no sea menor que 0
       
    }
    public void cambioColor()
    {
        string ICABueno = "#49E400";
        string ICAModerado = "#FEE004";
        string ICASensible = "#F27F00";
        string ICADanino = "#F30310";
        string ICAMuyDanino = "#A43F9D";
     
        

        
        if (ica >= 0 && ica <= 50)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICABueno, out color))
            {
                cirucloIca2.color = color; // Cambia el color del texto
              
            }
        }
        else if(ica >= 51 && ica <= 100)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAModerado, out color))
            {
                cirucloIca2.color = color; // Cambia el color del texto
               
            }
        }
        else if(ica >= 101 && ica <= 150) 
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICASensible, out color))
            {
                cirucloIca2.color = color; // Cambia el color del texto
              
            }
        }
        else if(ica >= 151 && ica <= 200) 
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICADanino, out color))
            {
                cirucloIca2.color = color; // Cambia el color del texto
              
            }
        }
        else if(ica >= 201 && ica <= 300) 
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAMuyDanino, out color))
            {
                cirucloIca2.color = color; // Cambia el color del texto
          
            }
        }
    }
}
