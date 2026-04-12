using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    [System.Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    // Input fields and message
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text errorMessage;
    // Buttons to activate after successful login
    public GameObject[] buttonsToActivate;
    public GameObject loginButon;

    private const string BASE_URL = "https://localhost:44356/api/auth/login";

    public async void ValidateInputs()
    {
        errorMessage.gameObject.SetActive(false); // ocultar mensaje antes de validar

        if (string.IsNullOrWhiteSpace(username.text) ||
            string.IsNullOrWhiteSpace(password.text))
        {
            errorMessage.text = "Por favor, rellena todos los campos requeridos.";
            errorMessage.gameObject.SetActive(true);

            return;
        }

        var loginResponse = await Login(username.text, password.text);

        if (!loginResponse.success)
        {
            errorMessage.text = loginResponse.errorMessage;
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // SceneManager.LoadScene(1);
        ActivarBotones();
    }

    private void ActivarBotones()
    {
        foreach (GameObject button in buttonsToActivate)
        {
            button.SetActive(true);
        }

        loginButon.SetActive(false);
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

        if (req.responseCode == 401 || req.responseCode == 404)
        {
            return (false, "Usuario no registrado o credenciales incorrectas.");
        }

        return (false, "No se pudo iniciar sesión. Inténtalo de nuevo.");
    }


}
