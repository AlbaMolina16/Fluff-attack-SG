using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Script asociado al prefab del panel Toast. Gestiona el contenido dinámico del mensaje
/// y su visualización temporizada. Puede reutilizarse en cualquier escena.
/// </summary>
public class ToastPanel : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private float duration = 3f;

    private Coroutine _hideCoroutine;

    /// <summary>
    /// Muestra el toast con el título y mensaje indicados.
    /// </summary>
    /// <param name="title">Texto del título del toast.</param>
    /// <param name="message">Texto del cuerpo del toast.</param>
    public void Show(string title, string message)
    {
        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;

        gameObject.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
