using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldNavigation : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields; // Campos que forman parte del formulario de creación de usuario
    [SerializeField] private Button submitButton; // SignUp button
    [SerializeField] private TMP_InputField passwordField; // Campo de la contraseña
    [SerializeField] private TMP_InputField confirmPasswordField; // Campo de la confirmación de contraseña
    [SerializeField] private DateInputField dateField; // Campo de la fecha de cumpleaños

    [SerializeField] private TMP_Text errorMessageText;
    public GameObject loadingSpinner;

    void Start()
    {
        // Poner el foco automáticamente en el primer campo de entrada
        FocusField(0);

        // Aunque esté deshabilitado el gameObject desde la interfaz de Unity nos aseguramos por código que también lo esté
        if (loadingSpinner != null)
            loadingSpinner.SetActive(false);

        // Suscribirse a los cambios de cada campo
        foreach (var field in inputFields)
            field.onValueChanged.AddListener(_ => ValidateFields());

        ValidateFields(); // Estado inicial
    }

    /// <summary>
    /// Detecta la pulsación de las teclas TAB y ENTER para navegar entre los campos de entrada y activar el botón de submit respectivamente.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                FocusPreviousField();
            else
                FocusNextField();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (submitButton.interactable)
                submitButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// Enfoca el siguiente campo de entrada en la lista. Si el último campo está enfocado, vuelve al primero.
    /// </summary>
    private void FocusNextField()
    {
        // Encontrar cuál campo está activo ahora
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                // Calcular el índice del siguiente (vuelve al primero si es el último)
                int nextIndex = (i + 1) % inputFields.Length;
                FocusField(nextIndex);
                return;
            }
        }

        // Si ninguno está enfocado, enfocar el primero
        FocusField(0);
    }

    /// <summary>
    /// Enfoca el campo de entrada anterior en la lista. Si el primer campo está enfocado, vuelve al último.
    /// </summary>
    private void FocusPreviousField()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                int prevIndex = (i - 1 + inputFields.Length) % inputFields.Length;
                FocusField(prevIndex);
                return;
            }
        }
        FocusField(inputFields.Length - 1);
    }

    /// <summary>
    /// Enfoca el campo de entrada en el índice especificado.
    /// </summary>
    /// <param name="index">El índice del campo de entrada a enfocar.</param>
    private void FocusField(int index)
    {
        inputFields[index].Select();
        inputFields[index].ActivateInputField();
    }

    /// <summary>
    /// Comprueba si todos los campos están informados y son correctos.
    /// </summary>
    private void ValidateFields()
    {
        bool enabled = true;

        foreach (var field in inputFields)
        {
            if (string.IsNullOrWhiteSpace(field.text))
            {
                enabled = false;
                break;
            }
        }

        bool passwordValid = PasswordsMatch();
        bool dateValid = dateField == null || dateField.IsValidDate();
        submitButton.interactable = enabled && passwordValid && dateValid;
    }

    /// <summary>
    /// Valida que los campos de contraseña y confirmación de contraseña coincidan. Si no coinciden, muestra un mensaje de error.
    /// </summary>
    /// <returns></returns>
    private bool PasswordsMatch()
    {
        if (passwordField == null || confirmPasswordField == null)
        {
            return true; // Si no se han asignado los campos de contraseña, no validamos esta condición
        }

        if (string.IsNullOrEmpty(passwordField.text) ||
            string.IsNullOrEmpty(confirmPasswordField.text))
        {
            errorMessageText.gameObject.SetActive(false);
            return false;
        }

        bool match = passwordField.text == confirmPasswordField.text;
        errorMessageText.gameObject.SetActive(!match);
        errorMessageText.text = "Las contraseñas no coinciden";
        return match;
    }

    public void ResetFields()
    {
        foreach (var field in inputFields)
            field.text = string.Empty;

        // Resetear el mensaje de error de contraseña si lo tienes
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);

        // Deshabilitar el botón submit
        submitButton.interactable = false;

        // Enfocar el primer campo
        FocusField(0);
    }
}