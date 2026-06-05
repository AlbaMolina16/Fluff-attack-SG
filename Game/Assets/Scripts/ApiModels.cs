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
    public UserInfo user;
}

[Serializable]
public class UserInfoResponse : UserInfo
{
    public UserPreferences preferences;
}

/// <summary>
/// Dto para mapear la información del usuario obtenida del API /auth/login
/// </summary>
[Serializable]
public class UserInfo
{
    public int id;
    public string nickname;
    public string firstName;
    public string lastName;
    public int age;
    public string handedness; // "diestro" o "zurdo"
    public int idDifficulty;
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
    public float gameTime = 120f; // Tiempo de juego en segundos
    public float spawnRate; // Enemigos por segundo
    public float enemyLifeTime; // Tiempo de vida de los enemigos en segundos
    public float enemySpeed; // Velocidad de los enemigos
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
    public int id = 0;
    public string name = "";
    public float gameTime = 120f; // Tiempo de juego en segundos
    public float enemySpeed = 0f; // Velocidad de los enemigos
    public float spawnRate = 0f; // Enemigos por segundo -> enemy/seg
    public float enemyLifeTime = 0f; // Tiempo de vida de los enemigos en segundos
    public int amountEnemies = 1; // Cantidad máxima de enemigos a la vez en pantalla
    public List<DifficultyMovement> movements = new() { new DifficultyMovement() }; // Tipos de movimientos que pueden tener los enemigos en esta dificultad con su probabilidad de aparecer
}

[Serializable]
public class DifficultyMovement
{
    public string name;
    public float probability; // Probabilidad del tipo de movimiento que seguirá una pelusa

    public DifficultyMovement()
    {
        name = "none";
        probability = 1f;
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