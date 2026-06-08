using UnityEngine;

/// <summary>
/// Maneja y ajusta la dificultad adaptativa (DDA) basandose en el rendimiento del jugador.
/// Ajusta los parametros de juego periodicamente para mantener un nivel terapeutico optimo.
/// ¿Qué vamos a medir?
/// - Precision global (aciertos / intentos totales) -> ajusta la dificultad general
/// - Precision por color -> ajusta el sesgo de aparicion de cada pelusa (rehabilitacion por dedo)
/// Parametros ajustados:
/// - spawnRate, amountEnemies, enemySpeed, enemyLifeTime
/// - Peso de aparicion por color [Rojo, Amarillo, Verde, Azul]
/// </summary>
public class AdaptiveDifficultyProvider : IDifficultyLevelProvider
{
    private readonly AdaptiveDifficultyConfig _config;

    /// <summary>
    /// Nivel de dificultad aplicado (entre 0-1)
    /// </summary>
    public float DifficultyLevel { get; private set; }

    // Pesos de aparicion por color: [Red=0, Yellow=1, Green=2, Blue=3]
    // Un peso mayor aumenta la frecuencia de ese color para forzar practica del dedo que mas lo necesita.
    private readonly float[] _colorWeights = { 1f, 1f, 1f, 1f };

    // Ventana de evaluacion
    private readonly float _windowDuration;
    private float _windowTimer; // Contador de segundos para ver si hay que evaluar una nueva ventana

    // Fotografía de las metricas al inicio de cada ventana
    private int _snapTotalShoots; // Foto del número total de disparos acertados
    private int _snapFails; // Foto del número total de fallos

    /// <summary>
    /// El array de cuadro posiciones representa [Red=0, Yellow=1, Green=2, Blue=3]
    /// </summary>
    private readonly int[] _snapColorShoots = new int[4];
    private readonly int[] _snapColorFails = new int[4];
    private readonly int[] _snapColorExpired = new int[4];

    // Penalizaciones suaves para evitar que el sesgo fatigue al jugador final.
    private const float ColorFailWeight = 0.60f;
    private const float ColorExpiredWeight = 0.35f;

    // Precision calculada en la ventana anterior (-1 = sin ventana previa)
    private float _prevWindowAccuracy = -1f;
    private readonly float[] _prevColorAccuracy = { -1f, -1f, -1f, -1f };

    /// <summary>
    /// Se interpola el spawnRate en función del nivel de dificultad para dar a cada nivel(paso) su valor correspondiente estre el min/max
    /// </summary>
    public float SpawnRate => Mathf.Lerp(_config.minSpawnRate, _config.maxSpawnRate, DifficultyLevel);
    /// <summary>
    /// Interpola la cantidad de enemigos al igual que el SpawnRate, pero al ser un entero redondea en caso de que el cálculo sea decimal
    /// </summary>
    public int AmountEnemies => Mathf.RoundToInt(Mathf.Lerp(_config.minAmountEnemies, _config.maxAmountEnemies, DifficultyLevel));
    public float EnemySpeed => Mathf.Lerp(_config.minEnemySpeed, _config.maxEnemySpeed, DifficultyLevel);
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
        float noneProb = Mathf.Lerp(0.90f, 0.00f, DifficultyLevel);
        float linealProb = Mathf.Lerp(0.10f, 0.55f, DifficultyLevel);

        if (rand < noneProb)
            return new DifficultyMovement { name = "none", probability = 1f };
        if (rand < noneProb + linealProb)
            return new DifficultyMovement { name = "lineal", probability = 1f };
        return new DifficultyMovement { name = "zigzag", probability = 1f };
    }

    /// <summary>
    /// Selecciona el indice de prefab de pelusa en función del peso que tiene asignado cada color
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

        // Calcular valores generados dentro de la propia ventana
        // Obtiene el número total del ScoreManager y le resta la foto de la ventana anterior
        int windowShoots = sm.TotalHits - _snapTotalShoots;
        int windowFails = sm.TotalFails - _snapFails;

        int[] windowColorShoots = new int[4];
        int[] windowColorFails = new int[4];
        int[] windowMissingFluffs = new int[4];

        windowColorShoots[0] = sm.RedFluffsCount - _snapColorShoots[0];
        windowColorShoots[1] = sm.YellowFluffsCount - _snapColorShoots[1];
        windowColorShoots[2] = sm.GreenFluffsCount - _snapColorShoots[2];
        windowColorShoots[3] = sm.BlueFluffsCount - _snapColorShoots[3];

        windowColorFails[0] = sm.RedFluffFails - _snapColorFails[0];
        windowColorFails[1] = sm.YellowFluffFails - _snapColorFails[1];
        windowColorFails[2] = sm.GreenFluffFails - _snapColorFails[2];
        windowColorFails[3] = sm.BlueFluffFails - _snapColorFails[3];

        windowMissingFluffs[0] = sm.MissingRedFluffs - _snapColorExpired[0];
        windowMissingFluffs[1] = sm.MissingYellowFluffs - _snapColorExpired[1];
        windowMissingFluffs[2] = sm.MissingGreenFluffs - _snapColorExpired[2];
        windowMissingFluffs[3] = sm.MissingBlueFluffs - _snapColorExpired[3];

        int windowExpired = windowMissingFluffs[0] + windowMissingFluffs[1] + windowMissingFluffs[2] + windowMissingFluffs[3];
        int windowTotal = windowShoots + windowFails + windowExpired;

        // Evaluamos la precisión de disparo general en la partida teniendo en cuenta las que no se ha conseguido eliminar y ha desaparecido
        float currentAccuracy = windowTotal > 0 ? (float)windowShoots / windowTotal : -1f;

        if (_prevWindowAccuracy >= 0f && currentAccuracy >= 0f)
        {
            bool prevHigh = _prevWindowAccuracy > _config.targetAccuracy + _config.accuracyTolerance;
            bool prevLow = _prevWindowAccuracy < _config.targetAccuracy - _config.accuracyTolerance;
            bool currHigh = currentAccuracy > _config.targetAccuracy + _config.accuracyTolerance;
            bool currLow = currentAccuracy < _config.targetAccuracy - _config.accuracyTolerance;

            // Si las dos ventanas consecutivas tienen un rendimiento por encima del óptimo, subimos la dificultad
            // y si ambas estan por debajo, la bajamos
            if (prevHigh && currHigh)
                DifficultyLevel = Mathf.Clamp01(DifficultyLevel + _config.difficultyStep);
            else if (prevLow && currLow)
                DifficultyLevel = Mathf.Clamp01(DifficultyLevel - _config.difficultyStep);
        }
        _prevWindowAccuracy = currentAccuracy;

        // Evaluamos por color la precisión que se está teniendo
        for (int i = 0; i < 4; i++)
        {
            // Rendimiento por color teniendo en cuenta aciertos, fallos de disparo y expiraciones.
            int colorRawSamples = windowColorShoots[i] + windowColorFails[i] + windowMissingFluffs[i];
            // Se minimiza la penalización por fallos de color y expiración para evitar fatiga
            float weightedPenalty = (windowColorFails[i] * ColorFailWeight) + (windowMissingFluffs[i] * ColorExpiredWeight);
            float weightedTotal = windowColorShoots[i] + weightedPenalty;
            float colorAcc = colorRawSamples >= _config.minColorAttemptsForBias && weightedTotal > 0f
                ? (float)windowColorShoots[i] / weightedTotal
                : -1f;

            if (_prevColorAccuracy[i] >= 0f && colorAcc >= 0f)
            {
                bool prevLow = _prevColorAccuracy[i] < _config.targetAccuracy - _config.accuracyTolerance;
                bool prevHigh = _prevColorAccuracy[i] > _config.targetAccuracy + _config.accuracyTolerance;
                bool currLow = colorAcc < _config.targetAccuracy - _config.accuracyTolerance;
                bool currHigh = colorAcc > _config.targetAccuracy + _config.accuracyTolerance;

                if (prevLow && currLow)
                    // Dedo con bajo rendimiento aumentamos un 25% la aparicion de ese color
                    _colorWeights[i] = Mathf.Clamp(_colorWeights[i] * 1.25f, 1f, 3f);
                else if (prevHigh && currHigh)
                    // Dedo con buen rendimiento sostenido: se normaliza el peso del color reduciendolo un 30% para que sea gradual
                    _colorWeights[i] = Mathf.Lerp(_colorWeights[i], 1f, 0.3f);
            }
            _prevColorAccuracy[i] = colorAcc;
        }

        TakeSnapshot();
    }

    private void TakeSnapshot()
    {
        var sm = ScoreManager.Instance;
        if (sm == null) return;

        _snapTotalShoots = sm.TotalHits;
        _snapFails = sm.TotalFails;

        _snapColorShoots[0] = sm.RedFluffsCount;
        _snapColorShoots[1] = sm.YellowFluffsCount;
        _snapColorShoots[2] = sm.GreenFluffsCount;
        _snapColorShoots[3] = sm.BlueFluffsCount;

        _snapColorFails[0] = sm.RedFluffFails;
        _snapColorFails[1] = sm.YellowFluffFails;
        _snapColorFails[2] = sm.GreenFluffFails;
        _snapColorFails[3] = sm.BlueFluffFails;

        _snapColorExpired[0] = sm.MissingRedFluffs;
        _snapColorExpired[1] = sm.MissingYellowFluffs;
        _snapColorExpired[2] = sm.MissingGreenFluffs;
        _snapColorExpired[3] = sm.MissingBlueFluffs;
    }
}
