
using System.Collections;
using UnityEngine;

/// <summary>
/// Scrip que gestiona la aparición de los paneles de login y registro, así como la visualización del panel toast de creación de usuario satisfactoriamente
/// </summary>
public class PanelManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject loginPanel; // Panel de login con usuario, contraseña, botón de login y botón de resgistro
    [SerializeField] private GameObject signUpPanel; // Panel de registro de usuario nuevo
    [SerializeField] private GameObject settingsPanel; // Panel de ajustes del usuario

    [Header("Panel Informativo")]
    [SerializeField] private ToastPanel toastPanel; // Componente del prefab toast

    // Referencia al script que gestiona la navegación entre los campos de entrada del panel para resetearlos
    [Header("Panel de navegación")]
    [SerializeField] private InputFieldNavigation loginNavigation;
    [SerializeField] private InputFieldNavigation signUpNavigator;

    private CanvasGroup _loginPanelCanvasGroup;

    void Start()
    {
        _loginPanelCanvasGroup = loginPanel.GetComponent<CanvasGroup>();
        if (_loginPanelCanvasGroup == null)
            _loginPanelCanvasGroup = loginPanel.AddComponent<CanvasGroup>();
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
    /// Muestra el panel de Ajustes y deshabilita las interacciones sobre el loginPanel
    /// </summary>
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
        _loginPanelCanvasGroup.interactable = false;
        _loginPanelCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Oculta el panel de Ajustes y restaura las interacciones sobre el loginPanel
    /// </summary>
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
        _loginPanelCanvasGroup.interactable = true;
        _loginPanelCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Muestra el panel toast con un título y mensaje dinámicos.
    /// </summary>
    /// <param name="title">Título del mensaje toast.</param>
    /// <param name="message">Cuerpo del mensaje toast.</param>
    public void ShowToast(string title, string message)
    {
        toastPanel.Show(title, message);
    }
}