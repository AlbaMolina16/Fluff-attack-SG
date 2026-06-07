using UnityEngine;

/// <summary>
/// Proveedor de dificultad adaptativa (DDA) basado en el rendimiento del jugador.
/// Ajusta los parametros de juego periodicamente para mantener un nivel terapeutico optimo.
///
/// Metricas monitorizadas:
/// - Precision global (aciertos / intentos totales) -> ajusta la dificultad general
/// - Precision por color -> ajusta el sesgo de aparicion de cada pelusa (rehabilitacion por dedo)
///
/// Parametros ajustados:
/// - spawnRate, amountEnemies, enemySpeed, enemyLifeTime
/// - Peso de aparicion por color [Rojo, Amarillo, Verde, Azul]
/// </summary>
public class AdaptiveDifficultyProvider : IDifficultyLevelProvider
{
    private readonly AdaptiveDifficultyConfig _config;

    /// <summary>Nivel de dificultad normalizado [0=facil, 1=dificil].</summary>
    public float DifficultyLevel { get; private set; }

    // Pesos de aparicion por color: [Red=0, Yellow=1, Green=2, Blue=3]
    // Un peso mayor aumenta la frecuencia de ese color para forzar practica del dedo que mas lo necesita.
    private readonly float[] _colorWeights = { 1f, 1f, 1f, 1f };

    // Ventana de evaluacion
    private readonly float _windowDuration;
    private float _windowTimer;

    // Snapshot de metricas al inicio de cada ventana
    private int _snapTotalHits;
    private int _snapFails;
    private readonly int[] _snapColorHits    = new int[4];
    private readonly int[] _snapColorExpired = new int[4];

    // --- IDifficultyLevelProvider ---
    public float SpawnRate    => Mathf.Lerp(_config.minSpawnRate, _config.maxSpawnRate, DifficultyLevel);
    public int   AmountEnemies => Mathf.RoundToInt(Mathf.Lerp(_config.minAmountEnemies, _config.maxAmountEnemies, DifficultyLevel));
    public float EnemySpeed   => Mathf.Lerp(_config.minEnemySpeed, _config.maxEnemySpeed, DifficultyLevel);
    public float EnemyLifeTime =>
        _config.enableEnemyLifeTime
            ? Mathf.Lerp(_config.maxEnemyLifeTime, _config.minEnemyLifeTime, DifficultyLevel)
            : 0f;

    public AdaptiveDifficultyProvider(AdaptiveDifficultyConfig config)
    {
        _config = config;
        DifficultyLevel = Mathf.Clamp01(config.initialDifficultyLevel);
        _windowDuration = config.evaluationWindowSeconds;
        TakeSnapshot();
    }

    /// <summary>
    /// Debe llamarse en cada Update del GamePlayManager con el deltaTime actual.
    /// </summary>
    public void Tick(float deltaTime)
    {
        _windowTimer += deltaTime;
        if (_windowTimer >= _windowDuration)
        {
            _windowTimer = 0f;
            EvaluateAndAdjust();
        }
    }

    /// <summary>
    /// Selecciona el tipo de movimiento segun el nivel de dificultad.
    /// A mayor dificultad, mayor probabilidad de enemigos con movimiento.
    /// </summary>
    public DifficultyMovement SelectMovement()
    {
        float rand = Random.value;
        float noneProb   = Mathf.Lerp(0.90f, 0.00f, DifficultyLevel);
        float linealProb = Mathf.Lerp(0.10f, 0.55f, DifficultyLevel);

        if (rand < noneProb)
            return new DifficultyMovement { name = "none",   probability = 1f };
        if (rand < noneProb + linealProb)
            return new DifficultyMovement { name = "lineal", probability = 1f };
        return new DifficultyMovement { name = "zigzag", probability = 1f };
    }

    /// <summary>
    /// Selecciona el indice de prefab de pelusa con sesgo adaptativo hacia los dedos con peor rendimiento.
    /// </summary>
    public int SelectFluffIndex(int count)
    {
        if (count != 4) return Random.Range(0, count);

        float total = _colorWeights[0] + _colorWeights[1] + _colorWeights[2] + _colorWeights[3];
        float rand = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < 4; i++)
        {
            cumulative += _colorWeights[i];
            if (rand <= cumulative) return i;
        }
        return 3;
    }

    // -------------------------------------------------------------------------

    private void EvaluateAndAdjust()
    {
        var sm = ScoreManager.Instance;
        if (sm == null) return;

        // Calcular deltas dentro de la ventana
        int windowHits  = sm.TotalHits - _snapTotalHits;
        int windowFails = sm.Fails      - _snapFails;

        int[] colorHitDeltas     = new int[4];
        int[] colorExpiredDeltas = new int[4];

        colorHitDeltas[0] = sm.RedFluffsCount    - _snapColorHits[0];
        colorHitDeltas[1] = sm.YellowFluffsCount - _snapColorHits[1];
        colorHitDeltas[2] = sm.GreenFluffsCount  - _snapColorHits[2];
        colorHitDeltas[3] = sm.BlueFluffsCount   - _snapColorHits[3];

        colorExpiredDeltas[0] = sm.MissingRedFluffs    - _snapColorExpired[0];
        colorExpiredDeltas[1] = sm.MissingYellowFluffs - _snapColorExpired[1];
        colorExpiredDeltas[2] = sm.MissingGreenFluffs  - _snapColorExpired[2];
        colorExpiredDeltas[3] = sm.MissingBlueFluffs   - _snapColorExpired[3];

        int windowExpired = colorExpiredDeltas[0] + colorExpiredDeltas[1] + colorExpiredDeltas[2] + colorExpiredDeltas[3];
        int windowTotal   = windowHits + windowFails + windowExpired;

        // --- Ajuste global de dificultad ---
        if (windowTotal >= _config.minAttemptsForEvaluation)
        {
            float accuracy = (float)windowHits / windowTotal;

            if (accuracy > _config.targetAccuracy + _config.accuracyTolerance)
                DifficultyLevel = Mathf.Clamp01(DifficultyLevel + _config.difficultyStep);
            else if (accuracy < _config.targetAccuracy - _config.accuracyTolerance)
                DifficultyLevel = Mathf.Clamp01(DifficultyLevel - _config.difficultyStep);
        }

        // --- Ajuste de sesgo por color (rehabilitacion especifica por dedo) ---
        for (int i = 0; i < 4; i++)
        {
            int colorTotal = colorHitDeltas[i] + colorExpiredDeltas[i];
            if (colorTotal < _config.minColorAttemptsForBias) continue;

            float colorAcc = (float)colorHitDeltas[i] / colorTotal;

            if (colorAcc < _config.targetAccuracy - _config.accuracyTolerance)
                // Dedo con bajo rendimiento: aumentar su presencia para forzar practica
                _colorWeights[i] = Mathf.Clamp(_colorWeights[i] * 1.25f, 1f, 3f);
            else if (colorAcc > _config.targetAccuracy + _config.accuracyTolerance)
                // Dedo con buen rendimiento: normalizar gradualmente su peso
                _colorWeights[i] = Mathf.Lerp(_colorWeights[i], 1f, 0.3f);
        }

        TakeSnapshot();
    }

    private void TakeSnapshot()
    {
        var sm = ScoreManager.Instance;
        if (sm == null) return;

        _snapTotalHits = sm.TotalHits;
        _snapFails     = sm.Fails;

        _snapColorHits[0] = sm.RedFluffsCount;
        _snapColorHits[1] = sm.YellowFluffsCount;
        _snapColorHits[2] = sm.GreenFluffsCount;
        _snapColorHits[3] = sm.BlueFluffsCount;

        _snapColorExpired[0] = sm.MissingRedFluffs;
        _snapColorExpired[1] = sm.MissingYellowFluffs;
        _snapColorExpired[2] = sm.MissingGreenFluffs;
        _snapColorExpired[3] = sm.MissingBlueFluffs;
    }
}
