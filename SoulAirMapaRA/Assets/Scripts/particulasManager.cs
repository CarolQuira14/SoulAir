using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class particulasManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public ParticleSystem cerca;
    [SerializeField] public ParticleSystem cerca2;
    [SerializeField] public ParticleSystem cerca3;

    [SerializeField] public Image fondo;
    [SerializeField] public Image info;
    [SerializeField] public Image cirucloAlrededor;
    [SerializeField] public Image cirucloIca, cirucloMapa, cirucloAvatar;

    public ICAManager iCAManager;
    public int icaP;
   

    // Update is called once per frame
    void Update()
    {
        cambioColor();
        cambioColor2();
        icaP = iCAManager.ica;
        //Debug.Log("Ica desde particulemanager" + icaP);
    }
    private void Start()
    {
        iCAManager=GetComponent<ICAManager>();
        
    }
    public void cambioColor()
    {
        var mainModule = cerca.main;
        var mainModule3 = cerca3.main;

        string ICABueno = "#49E400";
        string ICAModerado = "#FEE004";
        string ICASensible = "#F27F00";
        string ICADanino = "#F30310";
        string ICAMuyDanino = "#A43F9D";

        if (icaP >= 0 && icaP <= 50)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICABueno, out color))
            {
                mainModule.startColor = color;
                fondo.color = color;
                info.color = color;
                mainModule3.startColor = color;
            }
        }
        else if (icaP >= 51 && icaP <= 100)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAModerado, out color))
            {
                mainModule.startColor = color;
                fondo.color = color;
                info.color = color;
                mainModule3.startColor = color;
            }
        }
        else if (icaP >= 101 && icaP <= 150)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICASensible, out color))
            {
                mainModule.startColor = color;
                fondo.color = color;
                info.color = color;
                mainModule3.startColor = color;
            }
        }
        else if (icaP >= 151 && icaP <= 200)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICADanino, out color))
            {
                mainModule.startColor = color;
                fondo.color = color;
                info.color = color;
                mainModule3.startColor = color;
            }
        }
        else if (icaP >= 201 && icaP <= 300)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAMuyDanino, out color))
            {
                mainModule.startColor = color;
                fondo.color = color;
                info.color = color;
                mainModule3.startColor = color;
            }
        }
    }

    public void cambioColor2()
    {
        var mainModule2 = cerca2.main;

        string ICABueno = "#416332";
        string ICAModerado = "#D5C028";
        string ICASensible = "#9D6B34";
        string ICADanino = "#733B3D";
        string ICAMuyDanino = "#332D33";

        if (icaP >= 0 && icaP <= 50)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICABueno, out color))
            {
                mainModule2.startColor = color;
                cirucloAlrededor.color = color;
                cirucloIca.color = color;
                cirucloMapa.color = color;
                cirucloAvatar.color = color;

            }
        }
        else if (icaP >= 51 && icaP <= 100)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAModerado, out color))
            {
                mainModule2.startColor = color;
                cirucloAlrededor.color = color;
                cirucloIca.color = color;
                cirucloMapa.color= color;
                cirucloAvatar.color= color;
            }
        }
        else if (icaP >= 101 && icaP <= 150)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICASensible, out color))
            {
                mainModule2.startColor = color;
                cirucloAlrededor.color = color;
                cirucloIca.color = color;
                cirucloMapa.color = color;
                cirucloAvatar.color = color;
            }
        }
        else if (icaP >= 151 && icaP <= 200)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICADanino, out color))
            {
                mainModule2.startColor = color;
                cirucloAlrededor.color = color;
                cirucloIca.color = color;
                cirucloMapa.color = color;
                cirucloAvatar.color = color;
            }
        }
        else if (icaP >= 201 && icaP <= 300)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(ICAMuyDanino, out color))
            {
                mainModule2.startColor = color;
                cirucloAlrededor.color = color;
                cirucloIca.color = color;
                cirucloMapa.color = color;
                cirucloAvatar.color = color;
            }
        }
    }
}
