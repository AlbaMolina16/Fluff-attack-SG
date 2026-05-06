namespace FluffGameApi.Dtos
{
    public class UserPreferencesDto
    {
        public required int Id { get; set; }
        public required int IdDifficulty { get; set; }
        public required string DifficultyName { get; set; }
    }
}