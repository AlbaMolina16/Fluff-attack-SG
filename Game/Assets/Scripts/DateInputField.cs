using UnityEngine;
using TMPro;

public class DateInputField : MonoBehaviour
{
    private TMP_InputField dateField;

    void Start()
    {
        dateField = GetComponent<TMP_InputField>();

        if (dateField == null)
        {
            Debug.LogError("No se encontró TMP_InputField en este GameObject");
            return;
        }

        Debug.Log("InputField encontrado correctamente");
        dateField.onValueChanged.AddListener(OnValueChanged);
    }

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

    public bool IsValidDate()
    {
        return System.DateTime.TryParseExact(
            dateField.text,
            "dd/MM/yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out _
        );
    }
}