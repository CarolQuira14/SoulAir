using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class manejoPerfil : MonoBehaviour
{
    public TextMeshProUGUI usernametxt,useremailtxt;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(FirebaseAuthManager.userNameStatic);
    }

    // Update is called once per frame
    void Update()
    {
        useremailtxt.text = FirebaseAuthManager.emailUserStatic;
        usernametxt.text = FirebaseAuthManager.userNameStatic;
    }

    public void LogoutUser()
    {
        Debug.Log("Sesion cerrada correctamente");
        FirebaseAuthManager.auth.SignOut();
    }
}
