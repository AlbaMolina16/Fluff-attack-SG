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

    [System.Serializable]
    private class ErrorResponse
    {
        public string message;
    }

    // Input fields and message
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text errorMessage;
    // Buttons to activate after successful login
    public GameObject[] buttonsToActivate;
    public Button submitButton;
    public Button signUpButton;

    public GameObject loadingSpinner;

    // URL de la API en local
    private const string BASE_URL = "https://localhost:44356/api/auth/login";
    // URL de la API en Azure
    // private const string BASE_URL = "https://fluffgame.azurewebsites.net/api/auth/login";

    /// <summary>
    /// Realiza la validación de los campos de usuario y contraseña y, si son correctos, intenta iniciar sesión.
    /// </summary>
    public async void ValidateInputs()
    {
        errorMessage.gameObject.SetActive(false);

        loadingSpinner.SetActive(true);
        SetFieldsStatus(false);

        var loginResponse = await Login(username.text, password.text);
        loadingSpinner.SetActive(false);

        if (!loginResponse.success)
        {
            errorMessage.text = loginResponse.errorMessage;
            errorMessage.gameObject.SetActive(true);

            SetFieldsStatus(true);
            return;
        }

        OnLoginSuccess();
    }

    /// <summary>
    /// Muestra los botones de acciones permitidos al usuario, oculta el botón de Login y deshabilita los input de user y password
    /// </summary>
    private void OnLoginSuccess()
    {
        // Mostramos botones de acción
        foreach (GameObject button in buttonsToActivate)
        {
            button.SetActive(true);
        }

        // Ocultamos el botón de login
        submitButton.gameObject.SetActive(false);
        // Ocultamos el botón de registro
        signUpButton.gameObject.SetActive(false);
    }


    public async Task<(bool success, string errorMessage)> Login(string username, string password)
    {
        var payload = new LoginRequest
        {
            username = username,
            password = password
        };

        var json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest(BASE_URL, "POST");

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
