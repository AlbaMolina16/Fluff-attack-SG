using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Este script controla el temporizador de la partida.
/// Se encarga de actualizar el tiempo restante en la cabecera del juego para informar al usuario del tiempo restante para jugar.
/// </summary>
public class TimerManager : MonoBehaviour
{
    private float remainingTime; // Tiempo restante para jugar en segundos. Por defecto, 3 minutos.
    [SerializeField] private TMP_Text timerText; //GameObject de tipo texto donde se muestra la cuenta atrás.

    private bool running = false;
    private float elapsedTime = 0f; // Tiempo transcurrido en segundos desde el inicio del juego
    private int secondsCount = 0; // Contrador de segundos enteros transcurridos.

    void Start()
    {
        // Inicalizamos el texto con color rojo para indicar que no se ha iniciado el juego.
        remainingTime = UserSession.Instance.UserDifficulty != null ? UserSession.Instance.UserDifficulty.gameTime : 120f; // Si no hay dificultad seleccionada, se asigna el valor por defecto de 120 segundos.
        timerText.color = Color.red;
    }

    // Time.deltaTime es el tiempo en segundo que ha tardado en renderizarse el último frame
    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            if (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime; // Vamos restando tiempo al contador
                elapsedTime += Time.deltaTime; // Vamos sumando tiempo transcurrido por framwe

                if (DoSomethingWhenCheckSecond(elapsedTime))
                {
                    timerText.text = GetRemainingTimeString();
                }
            }
            else
            {
                StopTimer();
            }
        }
    }

    /// <summary>
    /// Inicia la cuenta atrás.
    /// </summary>
    public void StartTimer()
    {
        running = true;
        timerText.color = new Color32(0, 245, 255, 255);
        timerText.text = GetRemainingTimeString();
    }

    /// <summary>
    /// Paraliza la cuenta atrás.
    /// </summary>
    public void StopTimer()
    {
        running = false;
        remainingTime = 0f;
        timerText.color = Color.red;
    }

    public string GetRemainingTimeString()
    {
        return Mathf.CeilToInt(remainingTime).ToString() + " seg";
    }

    /// <summary>
    /// Devuelve la cuenta de los segundos transcurridos desde que se inició la cuenta atrás.
    /// </summary>
    /// <returns></returns>
    public int GetSecondsCount()
    {
        return secondsCount;
    }

    /// <summary>
    /// Devuelve el estado del temporizador, si está corriendo o no.
    /// </summary>
    /// <returns></returns>
    public bool IsRunning()
    {
        return running;
    }

    /// <summary>
    /// Función que indica si ha pasado un segundo entero desde la última vez que se comprobó.
    /// </summary>
    /// <returns></returns>
    public bool DoSomethingWhenCheckSecond(float timeToCheck)
    {
        int actualSeconds = (int)timeToCheck;
        if (actualSeconds != secondsCount)
        {
            secondsCount = actualSeconds;
            return true;
        }
        else
            return false;
    }
}