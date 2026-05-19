namespace FluffGameApi.Dtos
{
    public class DifficultyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal EnemySpeed { get; set; }
        public decimal EnemyLifeTime { get; set; }
        public decimal SpawnRate { get; set; }
        public int AmountEnemies { get; set; }
    }
}