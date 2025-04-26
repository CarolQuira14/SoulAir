using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class movementMapaNuevo : MonoBehaviour, IPointerClickHandler
{
    

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);

    }
}
