using UnityEngine;

/// <summary>
/// Interfaz de acceso a DifficultyLevelProvider 
/// </summary>
public interface IDifficultyLevelProvider
{
    float SpawnRate { get; }
    int AmountEnemies { get; }
    float EnemySpeed { get; }
    float EnemyLifeTime { get; }
    DifficultyMovement SelectMovement();
    int SelectFluffIndex(int count);
    Vector2 SelectSpawnPosition(Vector2 min, Vector2 max);
}