public static class ApiConfig
{
    private const string BASE_URL = "https://localhost:44356";
    // private const string BASE_URL = "https://fluffgame.azurewebsites.net";

    public static class Auth
    {
        public const string Login = BASE_URL + "/api/auth/login";
        public const string Register = BASE_URL + "/api/auth/register";
    }
}
