using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public float gameTime = 180f; // Tiempo total del juego = 3 minutos = 180 segundos
    [SerializeField] private TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
    }


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
