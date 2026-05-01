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
    public int idUsuario;
}
