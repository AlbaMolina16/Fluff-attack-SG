namespace FluffGameApi.Dtos
{
    public class RecentScoreResponseDto
    {
        public int TotalPoints { get; set; }
        public int IdDifficulty { get; set; }
        public string DifficultyName { get; set; } = string.Empty;
    }
}