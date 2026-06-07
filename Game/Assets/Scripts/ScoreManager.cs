using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Fluff;

/// <summary>
/// Clase Singleton para que almacene la puntuación obtenida.
/// TODO no se si realmente es necesario que fuera singleton, porque sólo se está utilizando aquí.
/// Pero no sé si sería útil a la hora de calcular la dificultad progresiva entre partidas
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private static int correctScore = 1;
    private static int failScore = -1;
    public int totalScore { get; private set; } = 0; // Puntos conseguidos durante el transcurso de la partida
    public int lastScore { get; private set; } = 0; // últimos puntos obtenidos
    public int redPoints { get; private set; } = 0;
    public int yellowPoints { get; private set; } = 0;
    public int bluePoints { get; private set; } = 0;
    public int greenPoints { get; private set; } = 0;
    public int missingPoints { get; private set; } = 0; // Puntos perdidos por no disparar a una pelusa a tiempo
    public int fails { get; private set; } = 0; // Número de fallos cometidos durante la partida

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

    public void AddOrSubstractPoints(bool isCorrect, EnemyType? color = null)
    {
        // Calculamos la puntuación total
        totalScore += isCorrect ? correctScore : failScore;
        // Alamacenamos la última puntuación (negativa o positiva)
        lastScore = isCorrect ? correctScore : failScore;

        // Si ha sido un disparo fallido, aumentamos el contador de fallos. Si no, sumamos los puntos en función del color para llevar un recuento.
        if (!isCorrect)
            fails++;
        else if (color.HasValue)
        {
            switch (color)
            {
                case EnemyType.Red:
                    redPoints += correctScore;
                    break;
                case EnemyType.Yellow:
                    yellowPoints += correctScore;
                    break;
                case EnemyType.Green:
                    greenPoints += correctScore;
                    break;
                case EnemyType.Blue:
                    bluePoints += correctScore;
                    break;
            }
        }
    }

    public void AddMissingPoints(int points)
    {
        missingPoints += points;
    }
    
    public void ClearScore()
    {
        totalScore = 0;
        lastScore = 0;
        redPoints = 0;
        yellowPoints = 0;
        bluePoints = 0;
        greenPoints = 0;
        fails = 0;
    }

}