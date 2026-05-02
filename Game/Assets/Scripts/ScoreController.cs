using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Controlador para cargar y mostrar las puntuaciones más recientes del usuario.
/// </summary>
public class ScoreController : MonoBehaviour
{
    /// <summary>
    /// Contenedor donde se muestran las puntuaciones
    /// </summary>
    [SerializeField] private LoadRecentScoresContainer scoresContainer;
    /// <summary>
    /// Objeto de texto para mostrar mensajes de error al cargar las puntuaciones
    /// </summary>
    public TMP_Text errorMessage;

    [Header("Última puntuación")]
    public TMP_Text totalPointsText;
    public TMP_Text redPointsText;
    public TMP_Text bluePointsText;
    public TMP_Text greenPointsText;
    public TMP_Text yellowPointsText;

    [System.Serializable]
    private class ScoreItem
    {
        public int totalPoints;
        public int idDifficulty;
        public string difficultyName;
    }

    [System.Serializable]
    private class RecentScoresResponse
    {
        public string message;
        public List<ScoreItem> scores;
    }

    [System.Serializable]
    private class LastScoreItem
    {
        public int totalPoints;
        public int redPoints;
        public int bluePoints;
        public int greenPoints;
        public int yellowPoints;
    }

    [System.Serializable]
    private class LastScoreResponse
    {
        public string message;
        public LastScoreItem score;
    }

    /// <summary>
    /// Al iniciar la escena, se cargan las puntuaciones más recientes y la última puntuación del usuario.
    /// </summary>
    private async void Start()
    {
        await LoadRecentScores();
        await LoadLastScore();
    }

    public async Task LoadRecentScores()
    {
        errorMessage.gameObject.SetActive(false);
        var url = $"{ApiConfig.Scores.Recent}?userId={UserSession.Instance.User?.id}";
        using var req = UnityWebRequest.Get(url);

        var operation = req.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
        {
            errorMessage.text = $"Error al obtener puntuaciones: {req.error}";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        var response = JsonUtility.FromJson<RecentScoresResponse>(req.downloadHandler.text);
        if (response?.scores == null) return;

        scoresContainer.SetScores(response.scores.Select(s => s.totalPoints));
    }

    public async Task LoadLastScore()
    {
        var url = $"{ApiConfig.Scores.Last}?userId={UserSession.Instance.User?.id}";
        using var req = UnityWebRequest.Get(url);

        var operation = req.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success) return;

        var response = JsonUtility.FromJson<LastScoreResponse>(req.downloadHandler.text);
        if (response?.score == null) return;

        var lastScore = response.score;
        if (totalPointsText) totalPointsText.text = lastScore.totalPoints.ToString("N0");
        if (redPointsText) redPointsText.text = lastScore.redPoints.ToString("N0");
        if (bluePointsText) bluePointsText.text = lastScore.bluePoints.ToString("N0");
        if (greenPointsText) greenPointsText.text = lastScore.greenPoints.ToString("N0");
        if (yellowPointsText) yellowPointsText.text = lastScore.yellowPoints.ToString("N0");
    }
}
