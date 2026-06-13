using System.Linq;
using UnityEngine;

/// <summary>
/// Clase para proveer de los parámetros de dificultad configurada por el jugador al GamePlayManager
/// </summary>
public class DifficultyLevelProvider : IDifficultyLevelProvider
{
    private readonly DifficultyOption _difficulty;

    public DifficultyLevelProvider(DifficultyOption difficulty)
    {
        _difficulty = difficulty;
    }

    public float SpawnRate => _difficulty.spawnRate;
    public int AmountEnemies => _difficulty.amountEnemies;
    public float EnemySpeed => _difficulty.enemySpeed;
    public float EnemyLifeTime => _difficulty.enemyLifeTime;

    /// <summary>
    /// Selecciona el indice de prefab de pelusa.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public int SelectFluffIndex(int count) => Random.Range(0, count);

    /// <summary>
    /// Devuelve la posicion de mundo donde debe aparecer la siguiente pelusa.
    /// Los parametros min/max definen los limites del area de juego.
    /// </summary>
    public Vector2 SelectSpawnPosition(Vector2 min, Vector2 max) =>
        new(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

    /// <summary>
    /// Devuelve el tipo de movimiento que se le asignara a la pelusa en funcion de la probabilidad acumulada 
    /// </summary>
    public DifficultyMovement SelectMovement()
    {
        if (_difficulty.movements == null || _difficulty.movements.Count == 0)
            return new DifficultyMovement();

        float total = _difficulty.movements.Sum(m => m.probability);
        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var m in _difficulty.movements)
        {
            cumulative += m.probability;
            if (rand <= cumulative)
                return m;
        }

        return new DifficultyMovement();
    }
}