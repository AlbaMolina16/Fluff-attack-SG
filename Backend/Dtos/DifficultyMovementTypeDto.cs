namespace FluffGameApi.Dtos
{
    public class DifficultyMovementTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Probability { get; set; }
        public decimal MinSpeed { get; set; }
        public decimal MaxSpeed { get; set; }
    }
}