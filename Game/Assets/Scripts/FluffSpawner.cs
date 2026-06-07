using UnityEngine;

/// <summary>
/// Script que se encarga de la gestión de spawn de la pelusas durante la partida
/// </summary>
public class FluffSpawner : MonoBehaviour
{
    [Header("Lista de prefabs de pelusas")]
    [Tooltip("Indica los tipos de pelusas que se pueden ir generando.")]
    [SerializeField] private GameObject[] fluffPrefabs;

    [Header("Contenedor de pelusas")]
    [Tooltip("Contenedor donde se instanciarán los prefabs de pelusas.")]
    [SerializeField] private Transform fluffsContainer;

    /// <summary>
    /// Dificultad de la partidad
    /// </summary>
    private IDifficultyLevelProvider _provider;
    private float _frequencyCount = 1f;
    private float _secondsTimer = 0f; // Cuenta de segundo en segundo para saber cuando hay que hacer spawn de una pelusa

    /// <summary>
    /// Se inicializa el spawner en función de la dificultad que le asigne el GamePlayManager.
    /// </summary>
    /// <param name="provider"></param>
    public void SetProvider(IDifficultyLevelProvider provider)
    {
        _provider = provider;
        _frequencyCount = 1f;
        _secondsTimer = 0f;
    }

    void Update()
    {
        if (_provider == null) return;

        _secondsTimer += Time.deltaTime;
        if (_secondsTimer >= 1f)
        {
            _secondsTimer -= 1f;
            _frequencyCount += _provider.SpawnRate;
            if (_frequencyCount >= 1f)
                SpawnFluff();
        }
    }

    /// <summary>
    /// Genera una nueva pelusa si no ha alcanzado el límite de pelusas en pantalla
    /// </summary>
    public void SpawnFluff()
    {
        if (_provider == null) return;
        if (fluffsContainer.childCount >= _provider.AmountEnemies) return;

        _frequencyCount -= 1f;

        var movement = _provider.SelectMovement();
        float x = Random.Range(LimitAreaGame.InstanceMinPantalla.x, LimitAreaGame.InstanceMaxPantalla.x);
        float y = Random.Range(LimitAreaGame.InstanceMinPantalla.y, LimitAreaGame.InstanceMaxPantalla.y);

        int idx = Random.Range(0, fluffPrefabs.Length);
        var fluff = Instantiate(fluffPrefabs[idx], new Vector3(x, y, 0), Quaternion.identity, fluffsContainer);

        var mc = fluff.GetComponent<MovementController>();
        if (mc != null)
            mc.InitMovevement(movement, _provider.EnemySpeed);
    }
}
