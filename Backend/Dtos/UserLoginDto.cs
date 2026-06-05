namespace FluffGameApi.Dtos
{
    public class UserLoginDto
    {
        public required int Id { get; set; }
        public required string Nickname { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int Age { get; set; }
        public required int IdDifficulty { get; set; }
    }
}