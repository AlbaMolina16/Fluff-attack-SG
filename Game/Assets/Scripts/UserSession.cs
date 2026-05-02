using UnityEngine;

/// <summary>
/// Singleton para almacenar la sesión del usuario actual.
/// </summary>
public class UserSession : MonoBehaviour
{
    /// <summary>
    /// Instancia única de acceso global
    /// </summary>
    public static UserSession Instance { get; private set; }

    public UserLoginResponse User { get; private set; }

    /// <summary>
    /// Asegura que solo exista una instancia de UserSession y que persista entre escenas.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Registra en la sesión el usuario actual
    /// </summary>
    /// <param name="user">Información del usuario</param>
    public void SetUser(UserLoginResponse user)
    {
        User = user;
    }

    /// <summary>
    /// Limpia la sesión del usuario.
    /// </summary>
    public void Clear()
    {
        User = null;
    }
}