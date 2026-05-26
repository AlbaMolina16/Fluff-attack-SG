using System;
using System.Collections.Generic;

[Serializable]
public class ErrorResponse
{
    public string message;
}

#region LOGIN Y USUARIO

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

#endregion

#region PREFERENCIAS DEL USUARIO

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

#endregion

#region DIFICULTAD

[Serializable]
public class DifficultyOption
{
    public int id;
    public string name;
    public float enemySpeed;
    public float spawnRate; // Enemigos por segundo -> enemy/seg
    public float enemyLifeTime;
    public int amountEnemies;
    public List<DifficultyMovement> movements;
}

[Serializable]
public class DifficultyMovement
{
    public string name;
    public float probability; // Probabilidad del tipo de movimiento que seguirá una pelusa
    public float minSpeed; // Mínima velocidad de la pelusa
    public float maxSpeed; // Mácima velocidad de la pelusa

    public DifficultyMovement()
    {
        name = "none";
        probability = 1f;
        minSpeed = 0f;
        maxSpeed = 0f;
    }
}

[Serializable]
public class DifficultiesResponse
{
    public string message;
    public DifficultyOption[] difficulties;
}

#endregion

#region PUNTUACIONES

[System.Serializable]
public class RecentScoreItem
{
    public int totalPoints;
    public int idDifficulty;
    public string difficultyName;
}

[System.Serializable]
public class RecentScoresResponse
{
    public string message;
    public List<RecentScoreItem> scores;
}

[System.Serializable]
public class LastScoreItem
{
    public int totalPoints;
    public int redPoints;
    public int bluePoints;
    public int greenPoints;
    public int yellowPoints;
    public int missingPoints;
}

[System.Serializable]
public class LastScoreResponse
{
    public string message;
    public LastScoreItem score;
}

[System.Serializable]
public class NewScoreRequest
{
    public int idUser;
    public int idDifficulty;
    public int redPoints;
    public int bluePoints;
    public int greenPoints;
    public int yellowPoints;
    public int missingPoints; // TODO No está implementado la contabilización de puntos que se han perdido porque no se llegó a disparar a una pelusa
    public int totalPoints;
}

#endregion