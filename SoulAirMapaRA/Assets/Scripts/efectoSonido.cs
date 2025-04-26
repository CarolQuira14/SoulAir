using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class efectoSonido : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] fabricas;
    AudioSource arbolesSource;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fabricas = GameObject.FindGameObjectsWithTag("fabrica");

    }

    public void sonarEfecto()
    {
        int fabricasCount = fabricas.Length;
        if (fabricasCount > 0)
        {
            Debug.Log("sonandoEfecto");
            arbolesSource.Play();
        }
    }
}
