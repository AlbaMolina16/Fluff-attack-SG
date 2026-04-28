
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel; // Panel de login con usuario, contraseña, botón de login y botón de resgistro
    [SerializeField] private GameObject signUpPanel; // Pandel de registro de usuario nuevo

    // Referencia al script que gestiona la navegación entre los campos de entrada del panel para resetearlos
    [SerializeField] private InputFieldNavigation loginNavigation;
    [SerializeField] private InputFieldNavigation signUpNavigator;


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
        signUpNavigator.ResetFields(); // Resetear los campos de entrada del panel
    }

    /// <summary>
    /// Muestra el panel de Registro
    /// </summary>
    public void ShowSignUp()
    {

        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);
        loginNavigation.ResetFields(); // Resetear los campos de entrada del panel
    }
}
