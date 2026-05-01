/// <summary>
/// Clase estática que almacena las uris de los endpoints del Api Rest
/// </summary>
public static class ApiConfig
{
    private const string BASE_URL = "https://localhost:44356";
    // private const string BASE_URL = "https://fluffgame.azurewebsites.net";

    public static class Auth
    {
        public const string Login = BASE_URL + "/api/auth/login";
        public const string Register = BASE_URL + "/api/auth/register";
    }

    public static class Scores
    {
        public const string Recent = BASE_URL + "/api/score/recent";
        public const string Last = BASE_URL + "/api/score/last";
    }
}