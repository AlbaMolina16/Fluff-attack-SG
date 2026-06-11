namespace FluffGameApi.Dtos
{
    public class UpdatePreferencesDto
    {
        public required int IdUser { get; set; }
        public required int IdDifficulty { get; set; }
    }
}
