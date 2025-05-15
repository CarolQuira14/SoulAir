using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInfoControls : MonoBehaviour
{
    public GameObject panelInfo;
    void Start()
    {
        var allObjects = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (var obj in allObjects)
        {
            if (obj.name == "Info")
            {
                panelInfo = obj.gameObject;
                break;
            }
        }
        //panelInfo = GameObject.Find("Info");
    }
    public void AbrirInfoPM()
    {
        //panelInfo = GameObject.Find("Info");

        panelInfo.SetActive(true);
    }
    public void CerrarInfoPM()
    {
        panelInfo.SetActive(false);
    }

}
