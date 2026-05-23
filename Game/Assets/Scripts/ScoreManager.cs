using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Fluff;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int totalScore { get; private set; } = 0; // Puntos conseguidos durante el transcurso de la partida
    public int lastScore { get; private set; } = 0; // últimos puntos obtenidos
    public int redPoints { get; private set; } = 0;
    public int yellowPoints { get; private set; } = 0;
    public int bluePoints { get; private set; } = 0;
    public int greenPoints { get; private set; } = 0;
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

    public void AddOrSubstractPoints(int points, EnemyType? color = null)
    {
        // Calculamos la puntuación total
        totalScore += points;
        // Alamacenamos la última puntuación (negativa o positiva)
        lastScore = points;

        // Si ha sido un disparo fallido, aumentamos el contador de fallos. Si no, sumamos los puntos en función del color para llevar un recuento.
        if (points < 0)
            fails++;
        else if (color.HasValue)
        {
            switch (color)
            {
                case EnemyType.Red:
                    redPoints += points;
                    break;
                case EnemyType.Yellow:
                    yellowPoints += points;
                    break;
                case EnemyType.Green:
                    greenPoints += points;
                    break;
                case EnemyType.Blue:
                    bluePoints += points;
                    break;
            }
        }
    }

}