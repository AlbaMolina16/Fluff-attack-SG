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

    public int UserId { get; private set; }
    public string Username { get; private set; }

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
    /// Registra en la sesión el identificador del usuario y el nickname
    /// </summary>
    /// <param name="userId">Identificador del usuario</param>
    /// <param name="username">Nick de usuario</param>
    public void SetUser(int userId, string username)
    {
        UserId = userId;
        Username = username;
    }

    /// <summary>
    /// Limpia la sesión del usuario.
    /// </summary>
    public void Clear()
    {
        UserId = 0;
        Username = string.Empty;
    }
}