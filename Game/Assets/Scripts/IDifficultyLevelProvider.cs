/// <summary>
/// Interfaz de acceso a DifficultyLevelProvider 
/// </summary>
public interface IDifficultyLevelProvider
{
    float SpawnRate { get; }
    int AmountEnemies { get; }
    float EnemySpeed { get; }
    DifficultyMovement SelectMovement();
}