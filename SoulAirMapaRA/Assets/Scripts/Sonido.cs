using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonido : MonoBehaviour
{
    GameObject[] arboles;

    AudioSource arbolesSource;
    // Start is called before the first frame update
    void Start()
    {
        arbolesSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        arboles = GameObject.FindGameObjectsWithTag("arbol");
        sonarMusica();
    }
    public void sonarMusica()
    {
        int treeCount = arboles.Length;
        if (treeCount > 0)
        {
            Debug.Log("sonandoCancion");
            arbolesSource.Play();
        }
    }
    
}
