using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpControlEsc_1 : MonoBehaviour
{
    
    [SerializeField] Animator anim;
    public void ShowPopUp()
    {
        
        gameObject.SetActive(true);
        anim.SetBool("show", true);
    }
    

    public void HidePopUp()
    {
        
        anim.SetBool("show", false);
    }

    void DisactiveGO()
    {
        gameObject.SetActive(false);
    }

}
