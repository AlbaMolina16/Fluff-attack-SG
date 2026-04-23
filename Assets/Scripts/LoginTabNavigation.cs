using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginTabNavigation : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public GameObject loadingSpinner;

    void Start()
    {
        // Poner el foco automáticamente en el campo usuario
        usernameField.Select();
        usernameField.ActivateInputField();

        // Aunque esté deshabilitado el gameObject desde la interfaz de Unity nos aseguramos por código que también lo esté
        if (loadingSpinner != null)
            loadingSpinner.SetActive(false);
    }

    void Update()
    {
        // Detecta la pulsación de la tecla TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Si estabamos en el input del username, pasamos al password
            if (usernameField.isFocused)
            {
                passwordField.Select();
                passwordField.ActivateInputField();
            }
            else if (passwordField.isFocused)
            {
                // Si estabamos en el input del password, pasamos al botón de login
                loginButton.Select();
            }
        }

        // Detecta la pulsación de la tecla ENTER
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // if (loginButton.gameObject == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            // {
                // loginButton.interactable = false; // Deshabilitar el botón para evitar múltiples clics
                loginButton.onClick.Invoke();
            // }
        }
    }
}