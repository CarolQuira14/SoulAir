using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerJuego : MonoBehaviour
{
    //Asignacion tipo Singleton para este Manager
    public static GameManagerJuego Instance { get; private set; }

    public int cantHumo = 0;
    public float salud = 100;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetHumo()
    {
        cantHumo++;
        //Monedas.text = cantMonedas.ToString();

        //txtPlayer.text = "Tengo " + cantMonedas.ToString() + " Monedas!";
    }
    public void MinusHumo()
    {
        cantHumo--;
    }

    public void SaludaAcaba(float salud)
    {
        if (this.salud <= 0)
        {
            //Poner el panel de Game Over aqui
        }
    }
    

}
