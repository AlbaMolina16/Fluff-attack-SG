
using System.Collections;
using UnityEngine;

/// <summary>
/// Scrip que gestiona la aparición de los paneles de login y registro, así como la visualización del panel toast de creación de usuario satisfactoriamente
/// </summary>
public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel; // Panel de login con usuario, contraseña, botón de login y botón de resgistro
    [SerializeField] private GameObject signUpPanel; // Panel de registro de usuario nuevo
    [SerializeField] private GameObject toastPanel; // Panel toast de información
    [SerializeField] private GameObject settingsPanel; // Panel de ajustes del usuario


    [Header("Toast")]
    [SerializeField] private float toastDuration = 3f;

    // Referencia al script que gestiona la navegación entre los campos de entrada del panel para resetearlos
    [Header("Input Navigation")]
    [SerializeField] private InputFieldNavigation loginNavigation;
    [SerializeField] private InputFieldNavigation signUpNavigator;

    private Coroutine _toastCoroutine;

    void Start()
    {
        ShowLogin(); // Pantalla inicial
    }

    /// <summary>
    /// Muestra el panel del Login
    /// </summary>
    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);
        settingsPanel.SetActive(false);
        signUpNavigator.ResetFields(); // Resetear los campos de entrada del panel
    }

    /// <summary>
    /// Muestra el panel de Registro
    /// </summary>
    public void ShowSignUp()
    {

        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);
        settingsPanel.SetActive(false);
        loginNavigation.ResetFields(); // Resetear los campos de entrada del panel
    }

    /// <summary>
    /// Muestra el panel de Ajustes
    /// </summary>
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Oculta el panel de Ajustes
    /// </summary>
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="success"></param>
    // public void ShowToast(string message, bool success)
    public void ShowToast()
    {
        if (_toastCoroutine != null)
            StopCoroutine(_toastCoroutine);

        // toastMessage.text = message;
        // toastBackground.color = success ? toastColorSuccess : toastColorError;
        toastPanel.SetActive(true);

        _toastCoroutine = StartCoroutine(HideToastAfterDelay());
    }

    private IEnumerator HideToastAfterDelay()
    {
        yield return new WaitForSeconds(toastDuration);
        toastPanel.SetActive(false);
    }
}