/// <summary>
/// Clase estática que almacena las URLS de los endpoints del Api Rest
/// </summary>
public static class ApiConfig
{
    // private const string BASE_URL = "https://localhost:44356/api";
    private const string BASE_URL = "https://fluffgame.azurewebsites.net/api";

    public static class Auth
    {
        public const string Login = BASE_URL + "/auth/login";
    }

    public static class User
    {
        public const string Register = BASE_URL + "/user/register";
        public const string Preferences = BASE_URL + "/user/preferences";
    }

    public static class Difficulty
    {
        public const string GetAll = BASE_URL + "/difficulty";
    }

    public static class Scores
    {
        public const string Recent = BASE_URL + "/score/recent";
        public const string Last = BASE_URL + "/score/last";
        public const string New = BASE_URL + "/score/new";
    }
}