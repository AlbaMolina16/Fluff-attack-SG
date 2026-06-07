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

    /// <summary>
    /// Devuelve el tipo de movimiento que se le asignará a la pelusa en función de la probabilidad acumulada 
    /// </summary>
    /// <returns></returns>
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