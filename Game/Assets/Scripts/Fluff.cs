using UnityEngine;

/// <summary>
/// Script que representa a las pelusas de colores en el juego
/// </summary>
public class Fluff : MonoBehaviour
{
    /// <summary>
    /// Tipos de pelusas
    /// </summary>
    public enum EnemyType { Red, Yellow, Green, Blue }

    /// <summary>
    /// Tipo de la pelusa, asignado en el inspector
    /// </summary>
    public EnemyType type;
}