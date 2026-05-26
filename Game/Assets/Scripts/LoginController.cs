using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [System.Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    [Header("Campos de entrada de usuario y contraseña")]
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;

    [Header("Mensaje informativo")]
    [SerializeField]
    private TMP_Text messageText;

    [Header("Botonera del jugador logeado")]
    [SerializeField]
    private GameObject playerButtonsContainer; // Se activan cuando el login del usuario sea correcto
    [SerializeField]
    private GameObject logoutButtonContainer;

    [Header("Botones de sesión y registro")]
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button signUpButton;

    [Header("Loading")]
    [SerializeField]
    private GameObject loadingSpinner;

    /// <summary>
    /// Al iniciar la escena, se comprueba si ya hay información del usuario en sesión, porque podría venir de la escena de puntuaciones.
    /// </summary>
    private void Start()
    {
        if (UserSession.Instance.User != null)
            OnLoginSuccess(true);
    }

    /// <summary>
    /// Realiza la validación de los campos de usuario y contraseña y, si son correctos, intenta iniciar sesión.
    /// </summary>
    public async void ValidateInputs()
    {
        messageText.gameObject.SetActive(false);

        loadingSpinner.SetActive(true);
        SetFieldsStatus(false);

        var loginResponse = await Login(username.text, password.text);
        loadingSpinner.SetActive(false);

        if (!loginResponse.success)
        {
            messageText.text = loginResponse.errorMessage;
            messageText.color = Color.red;
            messageText.gameObject.SetActive(true);

            SetFieldsStatus(true);
            return;
        }

        OnLoginSuccess();
    }

    /// <summary>
    /// Muestra los botones de acciones permitidos al usuario, oculta el botón de Login y deshabilita los input de user y password
    /// </summary>
    private void OnLoginSuccess(bool onStart = false)
    {
        if (onStart)
        {
            SetFieldsStatus(false);
            // Informamos el username y la password en los campos de la UI
            username.text = UserSession.Instance.User?.nickname;
            password.text = "********"; // Como ya ha iniciado sesion, no hace falta incluir el valor de la contraseña
        }

        // Mostramos botones de acción
        playerButtonsContainer.SetActive(true);

        // Ocultamos el botón de login
        submitButton.gameObject.SetActive(false);
        // Ocultamos el botón de registro
        signUpButton.gameObject.SetActive(false);
        // Mostramos el botón de desconexion
        logoutButtonContainer.gameObject.SetActive(true);
        // Mostramos mensaje de bienvenida
        messageText.color = Color.white;
        messageText.text = "¡Bienvenid@, " + UserSession.Instance.User.firstName + "!";
        messageText.gameObject.SetActive(true);
    }

    public async Task<(bool success, string errorMessage)> Login(string username, string password)
    {
        var payload = new LoginRequest
        {
            username = username,
            password = password
        };

        var json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest(ApiConfig.Auth.Login, "POST");

        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var operation = req.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
            // Almacenamos en sesion el id y nickname del usuario para poder acceder a ello desde otras escenas
            UserSession.Instance.SetUser(response.user);
            return (true, string.Empty);
        }

        if (req.responseCode == 401)
        {
            var error = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
            return (false, error?.message ?? "Credenciales incorrectas.");
        }

        return (false, "No se pudo iniciar sesión. Inténtalo de nuevo.");
    }

    /// <summary>
    /// Cierra la sesion del usuario, limpia los campos y restaura la UI al estado inicial.
    /// </summary>
    public void Logout()
    {
        UserSession.Instance.Clear();

        playerButtonsContainer.SetActive(false);

        logoutButtonContainer.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(true);
        signUpButton.gameObject.SetActive(true);

        username.text = string.Empty;
        password.text = string.Empty;
        SetFieldsStatus(true);

        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Al pasar las validaciones de campos, como se va a realizar una llamada a la API que puede tardar unos segundos,
    /// deshabilitamos los campos de usuario y contraseña para evitar que el usuario intente modificar los datos mientras se procesa la petición.
    /// </summary>
    private void SetFieldsStatus(bool interactable)
    {
        // Deshabilitar campos
        username.interactable = interactable;
        password.interactable = interactable;
        submitButton.interactable = interactable;
    }
}