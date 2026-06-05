using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// TODO: Me gustaría cambiarle el nombre al fichero, porque al final creo que lo utilizaré para cargar cosas iniciales y luego
// almacenar la información del usuario

/// <summary>
/// Singleton para almacenar la sesión del usuario actual.
/// </summary>
public class UserSession : MonoBehaviour
{
    /// <summary>
    /// Instancia única de acceso global
    /// </summary>
    public static UserSession Instance { get; private set; }

    /// <summary>
    /// Información personal del usuario.
    /// </summary>
    public UserInfo User { get; private set; }
    /// <summary>
    /// Prefencias de juego del usuario con respecto a la dificultad de los modos de juego.
    /// </summary>
    public DifficultyOption UserDifficulty { get; private set; }
    /// <summary>
    /// Indica las diferentes dificultades que se pueden elegir en el juego.
    /// </summary>
    public DifficultyOption[] Difficulties { get; private set; }


    private async void Start()
    {
        await LoadDifficulties();
    }

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
    /// Obtenemos las dificultades del juego desde el API y lo guardamos en sesión 
    /// </summary>
    /// <returns></returns>
    private async Task LoadDifficulties()
    {
        var result = await FetchDifficulties();

        if (result == null || result.Length == 0) return;
        SetDifficulties(result);
    }

    /// <summary>
    /// Obtiene la lista de dificultades desde el API. Si la llamada falla, devuelve null.
    /// </summary>
    /// <returns>Niveles de dificultad obtenidos</returns>
    public async Task<DifficultyOption[]> FetchDifficulties()
    {
        using var req = UnityWebRequest.Get(ApiConfig.Difficulty.GetAll);
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success) return null;

        var response = JsonUtility.FromJson<DifficultiesResponse>(req.downloadHandler.text);
        return response?.difficulties;
    }

    /// <summary>
    /// Registra en la sesión el usuario actual
    /// </summary>
    /// <param name="user">Información del usuario</param>
    public void SetUser(UserInfo user)
    {
        User = user;
        UserDifficulty = Difficulties.FirstOrDefault(x => x.id == user.idDifficulty);
    }

    public void UpdateUserPreferences(DifficultyOption difficulty)
    {
        UserDifficulty = difficulty;
    }

    public void SetDifficulties(DifficultyOption[] difficulties)
    {
        Difficulties = difficulties;
    }

    /// <summary>
    /// Limpia la sesión del usuario.
    /// </summary>
    public void Clear()
    {
        User = null;
    }
}