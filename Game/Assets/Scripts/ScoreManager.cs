using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text totalScoreText; //GameObject de tipo texto donde se muestra la puntuación obtenida.
    [SerializeField] private TMP_Text lastScoreText; // Indicará si el último disparo ha sido bueno o no

    private int score = 0; // Puntos conseguidos durante el transcurso de la partida

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPoints(int points)
    {
        score += points;
        totalScoreText.text = Mathf.FloorToInt(score).ToString();
        lastScoreText.color = Color.green;
        lastScoreText.text = "( + " + points + " )";
    }

    public void SubstractPoints(int points)
    {
        score -= points;
        totalScoreText.text = Mathf.FloorToInt(score).ToString();
        lastScoreText.color = Color.red;
        lastScoreText.text = "( - " + points + " )";
    }
}