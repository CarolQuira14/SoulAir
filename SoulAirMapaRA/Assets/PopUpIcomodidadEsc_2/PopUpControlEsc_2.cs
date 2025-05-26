using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpControlEsc_2 : MonoBehaviour
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
        Invoke(nameof(DisactiveGO), anim.GetCurrentAnimatorStateInfo(0).length);
    }

    void DisactiveGO()
    {
        gameObject.SetActive(false);
    }

}
