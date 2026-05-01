using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Script que gestiona la creación de puntuaciones recientes en la escena 'Scores'
/// </summary>
public class LoadRecentScoresContainer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TextMeshProUGUI scorePrefab; // Prefab para crear cada registro de puntuación
    [SerializeField] private Transform container; // Contenedor donde se instanciarán los prebas de puntuación

    [Header("Configuración")]
    [SerializeField] private int maxScores = 4; // Cantidad de registros de puntuación que se van a mostrar

    private readonly List<TextMeshProUGUI> _scoreTexts = new();

    public void SetScores(IEnumerable<int> scores)
    {
        ClearScores();

        if (scores.Count() == 0)
        {
            TextMeshProUGUI instance = Instantiate(scorePrefab, container);
            instance.text = "No ratings yet.";
            _scoreTexts.Add(instance);
            return;
        }
        
        foreach (int score in scores)
        {
            if (_scoreTexts.Count >= maxScores) break;

            TextMeshProUGUI instance = Instantiate(scorePrefab, container);
            instance.text = score.ToString("N0");
            _scoreTexts.Add(instance);
        }
    }

    public void AddScore(int score)
    {
        if (_scoreTexts.Count >= maxScores)
        {
            // Elimina el más antiguo
            Destroy(_scoreTexts[0].gameObject);
            _scoreTexts.RemoveAt(0);
        }

        TextMeshProUGUI instance = Instantiate(scorePrefab, container);
        instance.text = score.ToString("N0");
        _scoreTexts.Add(instance);
    }

    private void ClearScores()
    {
        foreach (var t in _scoreTexts)
            Destroy(t.gameObject);

        _scoreTexts.Clear();
    }
}
