using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Fluff;

/// <summary>
/// Clase Singleton para que almacene la puntuación obtenida.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    /// <summary>
    /// Puntos que suma al acertar en una pelusa. Se tiene en cuenta con valor uno para que se asemeje también a la cantidad
    /// </summary>
    private static int CorrectScore = 1;
    private static int FailScore = -CorrectScore;
    /// <summary>
    /// Puntos conseguidos durante el transcurso de la partida (tiene en cuenta los fallos) para visualizarlo en pantalla
    /// </summary>
    public int TotalScore { get; private set; } = 0;
    /// <summary>
    /// Ultimos puntos obtenidos (para hacerlo visual en pantalla)
    /// </summary>
    public int LastScore { get; private set; } = 0;

    public int RedFluffsCount { get; private set; } = 0; 
    public int YellowFluffsCount { get; private set; } = 0;
    public int BlueFluffsCount { get; private set; } = 0;
    public int GreenFluffsCount { get; private set; } = 0;
    public int MissingFluffsCount { get; private set; } = 0;
    public int Fails { get; private set; } = 0; // Número de fallos cometidos durante la partida
 
    // Pelusas perdidas por color (para el DDA adaptativo)
    public int MissingRedFluffs { get; private set; } = 0;
    public int MissingYellowFluffs { get; private set; } = 0;
    public int MissingGreenFluffs { get; private set; } = 0;
    public int MissingBlueFluffs { get; private set; } = 0;

    // Total de disparos acertados
    public int TotalHits => RedFluffsCount + YellowFluffsCount + GreenFluffsCount + BlueFluffsCount;

    private void Awake()
    {
        // Si no existe la instancia de ScoreManager, la asignamos y hacemos que no se destruya entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Añade o resta puntuación a la total en función de si el disparo es correcto o no.
    /// Si es disparo es correcto también lo tiene en cuenta para la cuenta de aciertos por color.
    /// </summary>
    /// <param name="isCorrect"></param>
    /// <param name="color"></param>
    public void AddOrSubstractPoints(bool isCorrect, EnemyType? color = null)
    {
        // Calculamos la puntuación total
        TotalScore += isCorrect ? CorrectScore : FailScore;
        // Alamacenamos la última puntuación (negativa o positiva)
        LastScore = isCorrect ? CorrectScore : FailScore;

        // Si ha sido un disparo fallido, aumentamos el contador de fallos. Si no, sumamos los puntos en función del color para llevar un recuento.
        if (!isCorrect)
            Fails++;
        else if (color.HasValue)
        {
            switch (color)
            {
                case EnemyType.Red:
                    RedFluffsCount += CorrectScore;
                    break;
                case EnemyType.Yellow:
                    YellowFluffsCount += CorrectScore;
                    break;
                case EnemyType.Green:
                    GreenFluffsCount += CorrectScore;
                    break;
                case EnemyType.Blue:
                    BlueFluffsCount += CorrectScore;
                    break;
            }
        }
    }

    /// <summary>
    /// Registra puntos perdidos por color (usada por el DDA para ajustar predisposicion por colores).
    /// </summary>
    public void AddMissingPointsByColor(EnemyType color)
    {
        MissingFluffsCount += 1;

        switch (color)
        {
            case EnemyType.Red: MissingRedFluffs++; break;
            case EnemyType.Yellow: MissingYellowFluffs++; break;
            case EnemyType.Green: MissingGreenFluffs++; break;
            case EnemyType.Blue: MissingBlueFluffs++; break;
        }
    }

    public void ClearScore()
    {
        TotalScore = 0;
        LastScore = 0;
        RedFluffsCount = 0;
        YellowFluffsCount = 0;
        BlueFluffsCount = 0;
        GreenFluffsCount = 0;
        Fails = 0;
        MissingFluffsCount = 0;
        MissingRedFluffs = 0;
        MissingYellowFluffs = 0;
        MissingGreenFluffs = 0;
        MissingBlueFluffs = 0;
    }
}