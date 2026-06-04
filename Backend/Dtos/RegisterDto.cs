namespace FluffGameApi.Dtos
{
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public required string Handedness { get; set; }
    }
}
