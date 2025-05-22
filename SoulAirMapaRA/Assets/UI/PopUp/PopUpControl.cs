using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Titulo, Direccion, Descripcion, Llamado;
    [SerializeField] Button boton1, boton2;
    [SerializeField] Animator anim;

    public void ShowPopUp(/*string titulo, */string direccion/*, string descripcion, string llamado, ,Action boton1CB, Action boton2CB*/)
    {
        /*Titulo.text = titulo;
        Direccion.text = direccion;
        Descripcion.text = descripcion;
        Llamado.text = llamado;*/
        Direccion.text = direccion;
        boton1.onClick.AddListener(() => HidePopUp());
        boton2.onClick.AddListener(() => HidePopUp());
        gameObject.SetActive(true);
        anim.SetBool("show", true);
    }

    public void HidePopUp()
    {
        boton1.onClick.RemoveAllListeners();
        boton2.onClick.RemoveAllListeners();
        anim.SetBool("show", false);
        Invoke(nameof(DisactiveGO), anim.GetCurrentAnimatorStateInfo(0).length);
    }

    void DisactiveGO()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /*
    [ContextMenu("PopUp Test")]
    void PopUpTest()
    {
        ShowPopUp("Mucha polucion", "En la calle 24a #21", "Se encuentra esceso un estado de calidad de aire inexistente", "Quieres hacer un reporte o jugar el minijuego", HidePopUp, HidePopUp);
    }*/
}
