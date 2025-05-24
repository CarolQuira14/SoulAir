using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField emailInput, passwordInput, usernameInput, emailLoginInput, passwordLoginInput; // Nuevo campo para el nombre de usuario

    public Text usernamee;
    public GameObject advertencia6letras;

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

    public void LoginUser()
    {
       
        auth.SignInWithEmailAndPasswordAsync(emailLoginInput.text, passwordLoginInput.text).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión: " + task.Exception);
                
            }
            else
            {
                AuthResult authResult = task.Result;
                user = authResult.User;
                Debug.Log(user.DisplayName);
                usernamee.text =  user.DisplayName; // Mostrar el nombre de usuario
            }
        });
    }

    public void LogoutUser()
    {
        auth.SignOut();
    }
}
