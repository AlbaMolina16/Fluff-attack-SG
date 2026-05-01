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

    /// <summary>
    /// Al iniciar la escena, se cargan las puntuaciones más recientes del usuario.
    /// </summary>
    private async void Start()
    {
        await LoadRecentScores();
    }

    public async Task LoadRecentScores()
    {
        errorMessage.gameObject.SetActive(false);
        var url = $"{ApiConfig.Scores.Recent}?userId={UserSession.Instance.UserId}";
        using var req = UnityWebRequest.Get(url);

        var operation = req.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
        {
            errorMessage.text = $"Error al obtener puntuaciones: {req.error}";
            errorMessage.gameObject.SetActive(true);

            // Debug.LogWarning($"Error al obtener puntuaciones: {req.error}");
            return;
        }

        var response = JsonUtility.FromJson<RecentScoresResponse>(req.downloadHandler.text);
        if (response?.scores == null) return;

        scoresContainer.SetScores(response.scores.Select(s => s.totalPoints));
    }
}