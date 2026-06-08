using UnityEngine;

/// <summary>
/// Configuracion del sistema de dificultad adaptativa (DDA).
/// </summary>
[System.Serializable]
public class AdaptiveDifficultyConfig
{
    [Header("Nivel de dificultad inicial [0=facil, 1=dificil]")]
    [Range(0f, 1f)] public float initialDifficultyLevel = 0.3f;

    [Header("Spawn Rate min/max (pelusas/seg)")]
    public float minSpawnRate = 0.2f;
    public float maxSpawnRate = 1.2f;

    [Header("Cantidad min/max de pelusas en pantalla")]
    public int minAmountEnemies = 1;
    public int maxAmountEnemies = 8;

    [Header("Velocidad min/max de las pelusas")]
    public float minEnemySpeed = 0.5f;
    public float maxEnemySpeed = 5f;

    [Header("Tiempo de vida de los enemigos")]
    [Tooltip("Si esta desactivado, las pelusas no desaparecen solas")]
    public bool enableEnemyLifeTime = true;
    [Tooltip("Tiempo de vida en seg (dificultad baja)")]
    public float maxEnemyLifeTime = 20f;
    [Tooltip("Tiempo de vida en seg (dificultad alta)")]
    public float minEnemyLifeTime = 3f;

    [Header("Tiempo de la ventana de analisis del rendimiento del jugador")]
    public float evaluationWindowSeconds = 15f;

    [Header("Parametros de ajuste")]
    [Tooltip("Rango óptimo de trabajo/esfuerzo [0-1]. Por encima se sube dificultad, por debajo se baja")]
    [Range(0f, 1f)]
    public float targetAccuracy = 0.70f; // Rango óptimo de rendimiento del jugador
    [Tooltip("Margen de error alrededor del objetivo antes de actuar")]
    [Range(0f, 0.3f)]
    public float accuracyTolerance = 0.10f;
    [Tooltip("Cuanto sube o baja el nivel de dificultad por ventana de evaluacion")]
    [Range(0.01f, 0.3f)]
    public float difficultyStep = 0.1f;
    [Tooltip("Minimo de shoots por color en la ventana para ajustar su sesgo de aparicion")]
    public int minColorAttemptsForBias = 3;
}