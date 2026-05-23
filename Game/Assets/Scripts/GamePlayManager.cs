using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    [Header("Textos de la cabecera")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Botonera final")]
    [SerializeField] private GameObject buttonsContainer;

    // [SerializeField] private Button scoreButton;
    // [SerializeField] private Button homeButton;




    private float frequencyCount = 1f; // Contabilidad la cantidad de pelusas por segundo
    private float secondsTimer = 0f; // Tiempo transcurrido en segundos desde el inicio del juego
    private bool gameStarted = false; // Indica si el juego a comenzado.

    // Start is called before the first frame update
    void Start()
    {
        //SpawnFluff();
        // buttonsContainer.SetActive(true);

    }

    // Update is called once per frame
    async void Update()
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
            gameStarted = true;
            titleText.text = "3, 2, 1... Vamos!";
            messageText.gameObject.SetActive(false);
        }
        else if (gameStarted && !timer.IsRunning())
        {
            titleText.text = "Se acabo!";
            gameStarted = false;
            // Hay que guardar la puntuación en BBDD.

            var result = await SaveScore();
            // homeButton.gameObject.SetActive(true);
            // scoreButton.gameObject.SetActive(true);
            buttonsContainer.SetActive(true);
            if (result.Item1)
            {
                messageText.text = "Puntuación guardada correctamente.";
            }
            else
            {
                messageText.text = result.Item2;
            }

            messageText.gameObject.SetActive(true);
            // messageText.text = "Pulsa espacio para volver a jugar.";
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            // }
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

    private async Task<(bool, string)> SaveScore()
    {
        NewScoreRequest newScore = new()
        {
            idUser = UserSession.Instance.User.id,
            idDifficulty = UserSession.Instance.UserDifficulty.id,
            redPoints = ScoreManager.Instance.redPoints,
            bluePoints = ScoreManager.Instance.bluePoints,
            greenPoints = ScoreManager.Instance.greenPoints,
            yellowPoints = ScoreManager.Instance.yellowPoints,
            missingPoints = null,
            totalPoints = ScoreManager.Instance.totalScore
        };

        var json = JsonUtility.ToJson(newScore);

        using var req = new UnityWebRequest(ApiConfig.Scores.New, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var operation = req.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        var textMessage = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
        return (req.result == UnityWebRequest.Result.Success, textMessage.message);
    }
}