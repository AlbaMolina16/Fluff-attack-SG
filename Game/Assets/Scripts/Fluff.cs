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

    /// <summary>
    /// Permite que el FluffSpawner establezca el tiempo de vida directamente.
    /// -1 = usar el valor de UserSession (comportamiento original).
    /// </summary>
    [HideInInspector] public float lifeTimeOverride = -1f;

    private SpriteRenderer sr;
    private Color fluffColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        fluffColor = sr.color;

        float lifeTime = lifeTimeOverride >= 0f
            ? lifeTimeOverride
            : (UserSession.Instance.UserDifficulty != null ? UserSession.Instance.UserDifficulty.enemyLifeTime : 0f);

        if (lifeTime > 0f)
            StartCoroutine(FadeOut(lifeTime));
    }

    /// <summary>
    /// Hace desaparecer a la pelusa de forma gradual
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator FadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            sr.color = new Color(fluffColor.r, fluffColor.g, fluffColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ScoreManager.Instance.AddMissingPointsByColor(type);
        Destroy(gameObject);
    }
}