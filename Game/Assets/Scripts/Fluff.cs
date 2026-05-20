using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Script que representa a las pelusas de colores en el juego
/// </summary>
public class Fluff : MonoBehaviour
{
    /// <summary>
    /// Tipos de pelusas
    /// </summary>
    public enum EnemyType { Red, Yellow, Green, Blue }

    /// <summary>
    /// Tipo de la pelusa, asignado en el inspector
    /// </summary>
    public EnemyType type;

    public float fadeDuration = 2f; // Tiempo que tarde en desaparecer
    private SpriteRenderer sr; // Componente Sprite Renderer del gameObject Fluff
    private Color fluffColor; // Color original de la pelusa

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        fluffColor = sr.color;
        // Comprobamos si hay configurado un tiempo de vida para las pelusas
        if (UserSession.Instance.UserDifficulty != null && UserSession.Instance.UserDifficulty.enemyLifeTime > 0)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // Esperamos el tiempo que se haya configurado en la dificultad para que la pelusa empiece a desaparecer
        float delay = UserSession.Instance.UserDifficulty.enemyLifeTime;
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            sr.color = new Color(fluffColor.r, fluffColor.g, fluffColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}