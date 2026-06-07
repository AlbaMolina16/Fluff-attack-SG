using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Script que orquesta la partida. Gestiona el inicio de la partida, el contador de puntos, 
/// </summary>
public class GamePlayManager : MonoBehaviour
{
    [Header("Gestor de spawn de pelusas")]
    [SerializeField] private FluffSpawner fluffSpawner;

    [Header("Gestor del contador de tiempo")]
    [SerializeField] private TimerManager timer;

    [Header("Textos de la cabecera")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Botonera final")]
    [SerializeField] private GameObject buttonsContainer;

    private bool gameStarted = false;
    /// <summary>
    /// Croshair del jugador
    /// </summary>
    private GameObject pointer;

    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("Player");
        EnablePointer(true);
        ScoreManager.Instance.ClearScore();
        fluffSpawner.enabled = false;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !timer.IsRunning())
        {
            fluffSpawner.SetProvider(new DifficultyLevelProvider(UserSession.Instance.UserDifficulty));
            fluffSpawner.enabled = true; // 
            fluffSpawner.SpawnFluff();

            timer.StartTimer();
            gameStarted = true;
            titleText.text = "3, 2, 1... Vamos!";
            messageText.gameObject.SetActive(false);
        }
        else if (gameStarted && !timer.IsRunning())
        {
            fluffSpawner.enabled = false;
            titleText.text = "Se acabo!";
            gameStarted = false;
            EnablePointer(false);

            var result = await SaveScore();
            buttonsContainer.SetActive(true);
            messageText.text = result.Item1 ? "Puntuacion guardada correctamente." : result.Item2;
            messageText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Guarda la puntuación de la partida al terminar.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Habilita o deshabilita el puntero del jugador (crosshair). Estará habilitado hasta que se termine la partida.
    /// </summary>
    /// <param name="enabled"></param>
    private void EnablePointer(bool enabled)
    {
        if (pointer != null)
            pointer.GetComponent<SpriteRenderer>().enabled = enabled;
    }
}