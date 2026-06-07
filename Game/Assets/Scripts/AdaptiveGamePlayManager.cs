using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Orquesta la partida en modo de dificultad ADAPTATIVA (DDA).
/// Separado de GamePlayManager para no afectar la logica de dificultades fijas.
/// </summary>
public class AdaptiveGamePlayManager : MonoBehaviour
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

    [Header("Panel de instrucciones")]
    [SerializeField] private GameObject instructionsPanel;

    [Header("Indicador de nivel de dificultad (opcional)")]
    [Tooltip("Si se asigna, mostrara el nivel de dificultad actual como porcentaje.")]
    [SerializeField] private TextMeshProUGUI difficultyLevelText;

    [Header("Configuracion DDA")]
    [SerializeField] private AdaptiveDifficultyConfig adaptiveConfig = new AdaptiveDifficultyConfig();

    private bool _gameStarted = false;
    private bool _gameFinished = false;
    private GameObject _pointer;
    private AdaptiveDifficultyProvider _adaptiveProvider;

    void Start()
    {
        _pointer = GameObject.FindGameObjectWithTag("Player");
        EnablePointer(false);
        ScoreManager.Instance.ClearScore();
        fluffSpawner.enabled = false;
        instructionsPanel.SetActive(true);
    }

    async void Update()
    {
        if (!_gameStarted && !_gameFinished && Input.GetKeyDown(KeyCode.Space) && !timer.IsRunning())
        {
            StartAdaptiveGame();
        }
        else if (_gameStarted && !_gameFinished)
        {
            _adaptiveProvider?.Tick(Time.deltaTime);
            UpdateDifficultyUI();

            if (!timer.IsRunning())
                await EndGame();
        }
    }

    private void StartAdaptiveGame()
    {
        _adaptiveProvider = new AdaptiveDifficultyProvider(adaptiveConfig);

        EnablePointer(true);
        instructionsPanel.SetActive(false);

        fluffSpawner.SetProvider(_adaptiveProvider);
        fluffSpawner.enabled = true;
        fluffSpawner.SpawnFluff();

        timer.StartTimer();
        _gameStarted = true;

        titleText.text = "3, 2, 1... Vamos!";
        messageText.gameObject.SetActive(false);
    }

    private async Task EndGame()
    {
        fluffSpawner.enabled = false;
        titleText.text = "Se acabo!";
        _gameFinished = true;
        EnablePointer(false);

        var result = await SaveScore();
        buttonsContainer.SetActive(true);
        messageText.text = result.Item1 ? "Puntuacion guardada correctamente." : result.Item2;
        messageText.gameObject.SetActive(true);
    }

    private void UpdateDifficultyUI()
    {
        if (difficultyLevelText == null || _adaptiveProvider == null) return;
        difficultyLevelText.text = $"Nivel: {Mathf.RoundToInt(_adaptiveProvider.DifficultyLevel * 100f)}%";
    }

    private async Task<(bool, string)> SaveScore()
    {
        NewScoreRequest newScore = new()
        {
            idUser        = UserSession.Instance.User.id,
            idDifficulty  = UserSession.Instance.UserDifficulty.id,
            redPoints     = ScoreManager.Instance.RedFluffsCount,
            bluePoints    = ScoreManager.Instance.BlueFluffsCount,
            greenPoints   = ScoreManager.Instance.GreenFluffsCount,
            yellowPoints  = ScoreManager.Instance.YellowFluffsCount,
            missingPoints = ScoreManager.Instance.MissingFluffsCount,
            totalPoints   = ScoreManager.Instance.TotalScore
        };

        var json = JsonUtility.ToJson(newScore);
        using var req = new UnityWebRequest(ApiConfig.Scores.New, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler   = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var operation = req.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        var textMessage = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
        return (req.result == UnityWebRequest.Result.Success, textMessage.message);
    }

    private void EnablePointer(bool enabled)
    {
        if (_pointer != null)
            _pointer.GetComponent<SpriteRenderer>().enabled = enabled;
    }
}
