using Firebase.Firestore;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class fireStorageListener : MonoBehaviour
{
    FirebaseFirestore db;
    ListenerRegistration listener; // Suscripción a Firestore
    string ciudad = "cali"; // Se puede cambiar según la ubicación del usuario
    public TMP_Text info; // info al usuario del valor del sensor qeu se muestra
    public static int icaPC = 1; // Último ICA conocido
    public static int icaUV = 1; // Último ICA conocido
    static int latitude; // latitud del sensor mostrado
    static int longitude; // longitud del sensor mostrado
    static DateTime hora; // hora del dato mostrado
    static string nombreSensor; // nombre del sensor mostrado
    public AirQualityCalculator airQualityCalculator;
    //static string nombreDocumento="pance"; // nombre del documento que se va a mostrar

    void Start()
    {
        //info = GameObject.Find("info_txt").GetComponent<TMP_Text>();
        //db.Settings = new FirebaseFirestoreSettings { PersistenceEnabled = true };
        //Debug.Log("Firestore Offline Habilitado");
        //ObtenerICA();
        db = FirebaseFirestore.DefaultInstance;
        SuscribirseACambios();
    }
    /*async Task ObtenerICA()
    {
        DocumentReference docRef = db.Collection("ICA").Document(ciudad);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            ultimoICA = snapshot.GetValue<int>("ICA");
            Debug.Log($"[Firestore] El ICA en {ciudad} es: {ultimoICA}");
        }
    }*/
    //public void escogerSensorUni()
    //{
    //    nombreDocumento = "univalle";
    //    info.text = "EL ICA en el sector de Univalle es de:";
    //    SuscribirseACambios();
    //}
    //public void escogerSensorPan()
    //{
    //    nombreDocumento = "pance";
    //    info.text = "EL ICA en el sector de Pance es de:";
    //    SuscribirseACambios();

    //}

    void SuscribirseACambios()
    {
        DocumentReference docRefPC = db.Collection("ICA").Document("pance");
        DocumentReference docRefUV = db.Collection("ICA").Document("univalle");

        listener = docRefPC.Listen(snapshot =>
        {
            if (snapshot.Exists)
            {

                // Verificar si el campo "ICA" existe antes de obtenerlo
                if (snapshot.ContainsField("value"))
                {
                    //airQualityCalculator.CalculateAverageICA();
                    int icaNuevoPC = snapshot.GetValue<int>("value");
                    latitude = snapshot.GetValue<int>("latitude");
                    longitude = snapshot.GetValue<int>("longitude");
                    hora = snapshot.GetValue<DateTime>("timestamp");  
                    nombreSensor = snapshot.GetValue<string>("name");

                    
                    if (icaNuevoPC != icaPC)
                    {
                        icaPC = icaNuevoPC;
                        
                        Debug.Log($"[Firestore] Nuevo ICA, a las: {hora} \n en {ciudad}, ubicado en {nombreSensor} es: {icaNuevoPC}");
                        
                    }
                }
                else
                {
                    Debug.LogWarning($"[Firestore] El documento en {ciudad} no contiene el campo 'value'.");
                }
            }
            else
            {
                Debug.LogWarning($"[Firestore] No se encontró el documento '{ciudad}' en la colección 'value'.");
            }
        });


        //UNIVALLE LISTENER

        listener = docRefUV.Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                // Verificar si el campo "ICA" existe antes de obtenerlo
                if (snapshot.ContainsField("value"))
                {
                    int icaNuevoUV = snapshot.GetValue<int>("value");
                    latitude = snapshot.GetValue<int>("latitude");
                    longitude = snapshot.GetValue<int>("longitude");
                    hora = snapshot.GetValue<DateTime>("timestamp");
                    nombreSensor = snapshot.GetValue<string>("name");

                    
                    if (icaNuevoUV != icaUV)
                    {
                        icaUV = icaNuevoUV;
                        //airQualityCalculator.CalculateAverageICA();
                        Debug.Log($"[Firestore] Nuevo ICA, a las: {hora} \n en {ciudad}, ubicado en {nombreSensor} es: {icaNuevoUV}");

                    }
                }
                else
                {
                    Debug.LogWarning($"[Firestore] El documento en {ciudad} no contiene el campo 'value'.");
                }
            }
            else
            {
                Debug.LogWarning($"[Firestore] No se encontró el documento '{ciudad}' en la colección 'value'.");
            }
        });

    }

    void OnDestroy()
    {
        // 🔹 Cancelar la suscripción al cerrar la app
        if (listener != null)
        {
            listener.Stop();
        }
    }
}
