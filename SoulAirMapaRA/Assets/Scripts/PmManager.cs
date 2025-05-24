using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PmManager : MonoBehaviour
{
    public GameObject particlePrefab;
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
        
    }
}
