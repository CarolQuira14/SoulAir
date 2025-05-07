using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class soporteApp : MonoBehaviour
{
    public string sceneName;
    public void AbrirSoportePorCorreo()
    {
        Application.OpenURL("mailto:soulAir@gmail.com?subject=Ayuda%20con%20la%20App&body=Describe%20tu%20problema%20aqu�");
    }

    // Cambiar a escena por nombre
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

}
