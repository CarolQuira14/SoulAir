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

    public Text usernamee;
    public GameObject advertencia6letras;
    public GameObject advertenciaVacioR,advertenciaVacioL;
    public GameObject advertenciaWrongPassword;
    public GameObject advertenciaInvalidEmail;
    public GameObject principal2;
    public GameObject iniciarSesion, registrarSesion;

    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase inicializado correctamente.");
            }
            else
            {
                Debug.LogError("No se pudieron resolver las dependencias de Firebase. Verifica tu conexión y configuración.");
                
            }
        });
    }

    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            advertenciaVacioR.SetActive(true);
            return;
        }

        if (password.Length < 6)
        {
            advertencia6letras.SetActive(true);
            return;
        }


        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error al registrar usuario: " + task.Exception);
            }
            else
            {
                AuthResult authResult = task.Result;
                user = authResult.User;

                // Actualizar el perfil del usuario con el nombre de usuario
                UpdateUserProfile(username);
                Debug.Log("Registrado con exito");
                iniciarSesion.SetActive(true);
                registrarSesion.SetActive(false);
            }
        });
    }

    private void UpdateUserProfile(string username)
    {
        if (user == null) return;  // Evita que se intente actualizar sin un usuario válido.

        UserProfile profile = new UserProfile { DisplayName = username };

        user.UpdateUserProfileAsync(profile).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Nombre de usuario actualizado: " + username);
            }
            else
            {
                Debug.LogError("Error al actualizar el nombre de usuario: " + task.Exception);
            }
        });
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

            Debug.Log("active y desactive");
            usernamee.text = user.DisplayName;
            Debug.Log(user.DisplayName + "hola, modificado");
            Debug.Log("Holaaaaaaaaaaaaaaaa");

            principal2.SetActive(true);
            iniciarSesion.SetActive(false);
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;

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


    public void LogoutUser()
    {
        auth.SignOut();
    }
}
