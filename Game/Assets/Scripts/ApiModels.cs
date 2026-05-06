using System;

[Serializable]
public class ErrorResponse
{
    public string message;
}

/// <summary>
/// Dto para mapear la respuesta del API /login
/// </summary>
[Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserLoginResponse user;
}

/// <summary>
/// Dto para mapear la información del usuario obtenida del API /auth/login
/// </summary>
[Serializable]
public class UserLoginResponse
{
    public int id;
    public string nickname;
    public string firstName;
    public string lastName;
    public DateTime birthday;
    public UserPreferences preferences;
}

/// <summary>
/// Dto para mapear las preferencias del usuario obtenidas del API /auth/login
/// </summary>
[Serializable]
public class UserPreferences
{
    public int id;
    public int idDifficulty;
    public string difficultyName;
}

[Serializable]
public class UpdatePreferencesRequest
{
    public int idDifficulty;
}

[Serializable]
public class DifficultyOption
{
    public int id;
    public string name;
}

[Serializable]
public class DifficultiesResponse
{
    public string message;
    public DifficultyOption[] difficulties;
}