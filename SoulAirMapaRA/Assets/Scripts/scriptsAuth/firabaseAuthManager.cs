using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FirebaseAuthManager : MonoBehaviour
{

    public InputField emailInput, passwordInput, usernameInput, emailLoginInput, passwordLoginInput; // Nuevo campo para el nombre de usuario

    public Text usernameBienvenida, exError;
    public GameObject advertencia6letras, shadow,confirmRegistroExitoso;
    public GameObject advertenciaVacioR,advertenciaVacioL, errorEx;
    public GameObject advertenciaWrongPassword;
    public GameObject advertenciaInvalidEmail;
    public GameObject principal2, principal1;
    public GameObject iniciarSesion, registrarSesion;

    public static FirebaseAuth auth;
    private FirebaseUser user;

    public static string userNameStatic, emailUserStatic;

    private async void Start()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase inicializado correctamente.");

            user = auth.CurrentUser;

            if (user != null)
            {

                Debug.Log("Usuario ya autenticado: " + user.Email);
                usernameBienvenida.text = user.DisplayName;
                guardarUsernameyCorreo(user.DisplayName, user.Email);

                principal2.SetActive(true);
                iniciarSesion.SetActive(false);
                registrarSesion.SetActive(false);
                principal1.SetActive(false);
                Debug.Log("ya estoy en la linea 49");
            }
            else
            {
                Debug.Log("No hay usuario autenticado, mostrar pantalla de login.");
                principal1.SetActive(true);
                principal2.SetActive(false);
                registrarSesion.SetActive(false);
                iniciarSesion.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("No se pudieron resolver las dependencias de Firebase.");
        }
    }

    public async void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            advertenciaVacioR.SetActive(true);
            shadow.SetActive(true);
            return;
        }

        if (password.Length < 6)
        {
            advertencia6letras.SetActive(true);
            shadow.SetActive(true);
            return;
        }

        try
        {
            var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = authResult.User;

            await UpdateUserProfile(username); // Espera a que termine la actualización del perfil
            Debug.Log("Registrado con éxito");

            confirmRegistroExitoso.SetActive(true);
            iniciarSesion.SetActive(true);
            registrarSesion.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al registrar usuario: " + ex.Message);

            errorEx.SetActive(true);
            shadow.SetActive(true);
            exError.text = ex.Message;

        }
    }
    private async System.Threading.Tasks.Task UpdateUserProfile(string username)
    {
        if (user == null) return;

        UserProfile profile = new UserProfile { DisplayName = username };

        try
        {
            await user.UpdateUserProfileAsync(profile);
            Debug.Log("Nombre de usuario actualizado: " + username);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al actualizar el nombre de usuario: " + ex.Message);
        }
    }

    public async void LoginUser()
    {
        if (string.IsNullOrEmpty(emailLoginInput.text) || string.IsNullOrEmpty(passwordLoginInput.text))
        {
            advertenciaVacioL.SetActive(true);
            return;
        }

        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(emailLoginInput.text, passwordLoginInput.text);
            user = authResult.User;

            //Debug.Log("active y desactive");
            usernameBienvenida.text = user.DisplayName;
            Debug.Log(user.DisplayName + " hola, bienvenido");
            Debug.Log("Holaaaaaaaaaaaaaaaa");

            guardarUsernameyCorreo(user.DisplayName, user.Email);

            principal2.SetActive(true);
            iniciarSesion.SetActive(false);
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.Log(errorCode + " este es el errorcode");
            Debug.Log(ex + " esta es la excepcion");

            switch (errorCode)
            {
                case AuthError.UserNotFound:
                    advertenciaInvalidEmail.SetActive(true);
                    break;
                case AuthError.WrongPassword:
                    advertenciaWrongPassword.SetActive(true);
                    break;
                default:
                    Debug.LogWarning($"Error desconocido: {ex.Message}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error inesperado: {ex.Message}");
        }
    }
    public void guardarUsernameyCorreo(string username, string email)
    {
        emailUserStatic = email;
        userNameStatic = username;
    }
}
