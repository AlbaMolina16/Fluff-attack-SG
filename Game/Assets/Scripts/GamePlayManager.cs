using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [Header("Lista de prefabs de pelusas")]
    [Tooltip("Indica los tipos de pelusas que se pueden ir generando.")]
    [SerializeField]
    private GameObject[] fluffPrefabs;

    [Header("Contenedor de pelusas")]
    [Tooltip("Es el contenedor donde se iran creado lo objetos de las pelusas.")]
    [SerializeField]
    private Transform fluffsContainer; // Contenedor donde se instanciarán los prefabs de pelusas

    [Header("Gestor del contador de tiempo")]
    [SerializeField]
    private TimerManager timer;

    private float frequencyCount = 1f; // Contabilidad la cantidad de pelusas por segundo
    private float secondsTimer = 0f; // Tiempo transcurrido en segundos desde el inicio del juego

    // Start is called before the first frame update
    void Start()
    {
        //SpawnFluff();
    }

    // Update is called once per frame
    void Update()
    {
        // Comprobamos que el temporizador esté corriendo para empezar a generar pelusas si aplica
        if (timer.IsRunning())
        {
            secondsTimer += Time.deltaTime; // Contabilizamos el tiempo transcurrido

            // Si ha pasado un segundo, comprobaremos si hay que mostrar pelusas en función del spawnRate
            if (secondsTimer >= 1f)
            {
                secondsTimer -= 1f;
                frequencyCount += UserSession.Instance.UserDifficulty.spawnRate;
                SpawnFluff();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !timer.IsRunning())
        {
            timer.StartTimer();
        }
    }

    /// <summary>
    /// Función para generar pelusas en función del spawnRate de la dificultad del usuario que está jugando
    /// y de si no tiene ya el número máximo de pelusas permitidas en la pantalla
    /// </summary>
    private void SpawnFluff()
    {
        if (frequencyCount >= 1f && UserSession.Instance.UserDifficulty != null && (fluffsContainer.childCount < UserSession.Instance.UserDifficulty.amountEnemies))
        {
            frequencyCount = 0f; // Reseteamos el contador de frecuencia de pelusas

            float x, y; // coordenadas x e y dentro de los límites de la pantalla donde se va a mostrar el fluff
            GetRandomPosition(out x, out y);
            int randomIndex = Random.Range(0, fluffPrefabs.Length); // Elegimos aleatoriamente un fluff de color
            GameObject fluff = Instantiate(fluffPrefabs[randomIndex], new Vector3(x, y, 0), Quaternion.identity, fluffsContainer);
        }
    }

    /// <summary>
    /// Función para obtener una posición aleatoria dentro de los límites de juego de la pantalla
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GetRandomPosition(out float x, out float y)
    {
        x = Random.Range(LimitAreaGame.InstanceMinPantalla.x, LimitAreaGame.InstanceMaxPantalla.x);
        y = Random.Range(LimitAreaGame.InstanceMinPantalla.y, LimitAreaGame.InstanceMaxPantalla.y);
    }
}