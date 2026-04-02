using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    // Input fields and message
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text errorMessage;
    // Buttons to activate after successful login
    public GameObject[] buttonsToActivate;
    public GameObject loginButon;

    public void ValidateInputs()
    {
        // Aquí puedes agregar la lógica para validar los inputs del usuario
        // Por ejemplo, verificar si el nombre de usuario y la contraseña son correctos
        errorMessage.gameObject.SetActive(false); // ocultar mensaje antes de validar

        if (string.IsNullOrWhiteSpace(username.text) ||
            string.IsNullOrWhiteSpace(password.text))
        {
            errorMessage.text = "Por favor, rellena todos los campos requeridos.";
            errorMessage.gameObject.SetActive(true);
            
            return;
        }

        // Todos los campos están completos
        Debug.Log("Campos válidos. Continuando...");
        // Si la validación es exitosa, activa los botones
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

}
