using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GSPLocation : MonoBehaviour
{
    public TextMeshProUGUI GPSStatus;
    public TextMeshProUGUI latitudeValue;
    public TextMeshProUGUI longitudeValue;
    public TextMeshProUGUI altitudeValue;
    public TextMeshProUGUI horizontalAccuracyValue;
    public TextMeshProUGUI timestampValue;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        if (!Input.location.isEnabledByUser)
        {
            Input.location.Start();
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
            }

            if (maxWait < 1)
            {
                GPSStatus.text = "Isnt really working bitch time out";
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                GPSStatus.text = "Isnt really working bitch unable to determinate device location";
                yield break;
            }
            else
            {
                GPSStatus.text = "Running gyal";
                InvokeRepeating("UpdateGPSData", 0.5f, 1f);
            }
        }
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            GPSStatus.text = "Running gyal";
            latitudeValue.text = Input.location.lastData.latitude.ToString();
            longitudeValue.text = Input.location.lastData.longitude.ToString();
            altitudeValue.text = Input.location.lastData.altitude.ToString();
            horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
            timestampValue.text = Input.location.lastData.timestamp.ToString();
        }
        else
        {
            GPSStatus.text = "Stop bro";
        }
    }
}
