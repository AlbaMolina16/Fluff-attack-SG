using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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

    private float frequencyCount = 1f; // Contabilidad la cantidad de pelusas por segundo
    private float secondsTimer = 0f; // Tiempo transcurrido en segundos desde el inicio del juego
    private bool gameStarted = false; // Indica si el juego a comenzado.
    private GameObject pointer;

    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("Player");
        EnablePointer(true);
        ScoreManager.Instance.ClearScore();
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
            SpawnFluff();
        }
        else if (gameStarted && !timer.IsRunning())
        {
            titleText.text = "Se acabo!";
            gameStarted = false;
            EnablePointer(false);
            // Hay que guardar la puntuación en BBDD.
            var result = await SaveScore();

            buttonsContainer.SetActive(true);
            if (result.Item1)
            {
                messageText.text = "Puntuacion guardada correctamente.";
            }
            else
            {
                messageText.text = result.Item2;
            }

            messageText.gameObject.SetActive(true);
        }
    }

    #region MANAGE SPAWN
    /// <summary>
    /// Función para generar pelusas en función del spawnRate de la dificultad del usuario que está jugando
    /// y de si no tiene ya el número máximo de pelusas permitidas en la pantalla
    /// </summary>
    private void SpawnFluff()
    {
        if (frequencyCount >= 1f && UserSession.Instance.UserDifficulty != null && (fluffsContainer.childCount < UserSession.Instance.UserDifficulty.amountEnemies))
        {
            frequencyCount = 0f; // Reseteamos el contador de frecuencia de pelusas

            // Seleccionamos el tipo de movimiento
            var movement = SelectProbabilityMovement() ?? new DifficultyMovement();

            // Obtenemos una posición aleatoria dentro de los límites de la pantalla para mostrar la pelusa
            GetRandomPosition(out float x, out float y);
            // TODO Igual estaría bien tener en cuenta los fallos que ha podido tener a lo largo de una partida sobre un color en concreto
            int randomIndex = Random.Range(0, fluffPrefabs.Length); // Elegimos aleatoriamente un fluff de color
            GameObject fluff = Instantiate(fluffPrefabs[randomIndex], new Vector3(x, y, 0), Quaternion.identity, fluffsContainer);

            var movementController = fluff.GetComponent<MovementController>();
            if (movementController != null)
                movementController.InitMovevement(movement);
        }
    }

    /// <summary>
    /// Se le asigna a la pelusa un tipo de movimiento en función de la probabilidad configurada
    /// El movimiento viene dado por el tipo y la velocidad con la que se va realizar
    /// </summary>
    private DifficultyMovement SelectProbabilityMovement()
    {
        // PROBABILIDAD ACUMULADA
        // Calculamos la probabilidad total por si no sumaran 1, así generamos el número aleatorio entr 0 y la suma total
        float totalProbability = 0f;
        if (UserSession.Instance.UserDifficulty != null && UserSession.Instance.UserDifficulty.movements.Count > 0)
        {
            totalProbability = UserSession.Instance.UserDifficulty.movements.Sum(m => m.probability);
            // Generamos el número aleatorio entre 0 y la probabilidad total
            float randomValue = Random.Range(0f, totalProbability);
            // Tenemos que superar con el rango de probabilidad de cada movimiento el número aleatorio generado
            float cumulativeProbability = 0f;

            foreach (var movement in UserSession.Instance.UserDifficulty.movements)
            {
                cumulativeProbability += movement.probability;
                if (randomValue <= cumulativeProbability)
                {
                    // Asignar el tipo de movimiento a la pelusa
                    return movement;
                }
            }

            return null; // Si no se selecciona ningún movimiento, devolvemos null
        }
        else
            return null; // Si no hay movimientos configurados, devolvemos null para que la pelusa no tenga movimiento asignado

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
    #endregion

    #region MANAGE SCORE
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
            missingPoints = ScoreManager.Instance.missingPoints,
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
    #endregion

    /// <summary>
    /// Habilita o deshabilita el sprite del Crosshair
    /// </summary>
    /// <param name="enabled"></param>
    private void EnablePointer(bool enabled)
    {
        if (pointer != null)
        {
            pointer.GetComponent<SpriteRenderer>().enabled = enabled;
        }
    }
}