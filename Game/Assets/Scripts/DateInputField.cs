using UnityEngine;
using TMPro;

/// <summary>
/// Script que gestiona el campo de entrada de fecha
/// </summary>
public class DateInputField : MonoBehaviour
{
    private TMP_InputField dateField;

    void Start()
    {
        dateField = GetComponent<TMP_InputField>();

        if (dateField == null)
        {
            // Debug.LogError("No se encontró TMP_InputField en este GameObject");
            return;
        }

        // Debug.Log("InputField encontrado correctamente");
        dateField.onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// Formatea la entrada del usuario para que siga el formato DD/MM/YYYY, permitiendo solo números y agregando las barras automáticamente. También mueve el cursor al final del texto después de formatear.
    /// </summary>
    /// <param name="value">El valor actual del campo de entrada</param>
    private void OnValueChanged(string value)
    {
        // Eliminar caracteres no numéricos
        string digits = "";
        foreach (char c in value)
            if (char.IsDigit(c)) digits += c;

        // Aplicar formato DD/MM/YYYY
        string formatted = "";
        for (int i = 0; i < digits.Length && i < 8; i++)
        {
            if (i == 2 || i == 4) formatted += "/";
            formatted += digits[i];
        }

        // Actualizar sin disparar el evento de nuevo
        dateField.onValueChanged.RemoveListener(OnValueChanged);
        dateField.text = formatted;

        // Mover el cursor al final del texto
        dateField.stringPosition = formatted.Length;
        dateField.caretPosition = formatted.Length;

        dateField.onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// Valida si la fecha ingresada es correcta según el formato DD/MM/YYYY y representa una fecha válida.
    /// <returns>true si la fecha es válida, false en caso contrario</returns>
    /// </summary>
    public bool IsValidDate()
    {
        if (dateField == null)
        {
            // Debug.LogError("No se encontró TMP_InputField en este GameObject");
            return false;
        }

        return System.DateTime.TryParseExact(
            dateField.text,
            "dd/MM/yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out _
        );
    }

    /// <summary>
    /// Devuelve la fecha ingresada. La fecha se espera en formato DD/MM/YYYY.
    /// </summary>
    /// <returns>Un objeto DateTime si la fecha es válida, o null si no lo es.</returns>
    public System.DateTime? GetDate()
    {
        if (System.DateTime.TryParseExact(
            dateField.text,
            "dd/MM/yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out System.DateTime result))
            return result;
        return null;
    }
}