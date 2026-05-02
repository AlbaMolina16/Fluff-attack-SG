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
/// Dto para mapear la respuesta del API /login
/// </summary>
[Serializable]
public class UserLoginResponse
{
    public int id;
    public string nickname;
    public string firstName;
    public string lastName;
    public DateTime birthday;
}