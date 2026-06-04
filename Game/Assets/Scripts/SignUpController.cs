using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignUpController : MonoBehaviour
{
    [Header("Campos de entrada a enviar")]
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private TMP_InputField firstNameField;
    [SerializeField] private TMP_InputField lastNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField ageField;
    [SerializeField] private TMP_Dropdown handDropdown;

    [Header("Mensaje de error")]
    [SerializeField] private TMP_Text errorMessageText;
    [SerializeField] private PanelManager panelManager;

    public GameObject loadingSpinner;

    public List<Selectable> _formFields; // Almaceno los elementos de tipo texto o botón para habilitarnos o no en función del estado de la llamada

    [Serializable]
    private class RegisterRequest
    {
        public string username;
        public string password;
        public string firstName;
        public string lastName;
        public int age;
        public string handedness; // "diestro" o "zurdo"
    }

    public async void OnSignUp()
    {
        errorMessageText.gameObject.SetActive(false);
        loadingSpinner.SetActive(true);
        SetFieldsInteractable(false);

        var result = await SendRegisterRequest();

        loadingSpinner.SetActive(false);

        if (!result.success)
        {
            errorMessageText.text = result.errorMessage;
            errorMessageText.gameObject.SetActive(true);
            SetFieldsInteractable(true);
            return;
        }

        OnSignUpSuccess();
    }

    private async Task<(bool success, string errorMessage)> SendRegisterRequest()
    {
        var payload = new RegisterRequest
        {
            username = nicknameField.text,
            password = passwordField.text,
            firstName = firstNameField.text,
            lastName = lastNameField.text,
            age = ageField.text.Length > 0 ? int.Parse(ageField.text) : 0,
            handedness = handDropdown.options[handDropdown.value].text
        };

        var json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest(ApiConfig.User.Register, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var operation = req.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (req.result == UnityWebRequest.Result.Success)
            return (true, string.Empty);

        if (req.responseCode == 409)
        {
            var error = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
            return (false, error?.message ?? "El usuario ya está registrado.");
        }

        return (false, "No se pudo completar el registro. Inténtalo de nuevo.");
    }

    /// <summary>
    /// Si el registro ha ido bien, muestra el panel de login y un mensaje toast de éxito al crear el usuario.
    /// También habilita los campos de entrada y botones en caso de que se hubieran deshabilitado por la llamada.
    /// </summary>
    private void OnSignUpSuccess()
    {
        if (panelManager != null)
        {
            panelManager.ShowLogin();
            panelManager.ShowToast("¡Registro completado!", "El usuario se ha creado correctamente.");
        }

        SetFieldsInteractable(true);
    }

    /// <summary>
    /// Habilita o deshabilita los campos de entrada y botones para evitar múltiples interacciones
    /// </summary>
    /// <param name="interactable"> = true -> habilitado, = false -> deshabilitado</param>
    private void SetFieldsInteractable(bool interactable)
    {
        foreach (var field in _formFields)
            field.interactable = interactable;
    }
}