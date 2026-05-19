using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Este scrip controla el temporizado del tiempo de la cabecera del juego para informar al usuario del tiempo restante para jugar.
/// </summary>
public class TimerManager : MonoBehaviour
{
    public float gameTime = 180f; // Tiempo total del juego = 3 minutos = 180 segundos
    [SerializeField] private TMP_Text timerText;

    // Update is called once per frame
    void Update()
    {
        gameTime -= Time.deltaTime; // Vamos restando segundos
        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        timerText.text = Mathf.FloorToInt(gameTime).ToString() + " seg";
    }
}