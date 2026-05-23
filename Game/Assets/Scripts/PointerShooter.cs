using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Fluff;

/// <summary>
/// Este script se encarga de detectar si se está apuntando a un enemido y si se está pulsando la tecla indicada para eliminarlo.
/// También va añadiendo puntos por cada enemigo eliminado
/// </summary>
public class PointerShooter : MonoBehaviour
{
    [Header("Textos de puntuación")]
    [SerializeField] private TMP_Text totalScoreText; //GameObject de tipo texto donde se muestra la puntuación obtenida.
    [SerializeField] private TMP_Text lastScoreText; // Indicará si el último disparo ha sido bueno o no

    [Header("Clips de audio")]
    [SerializeField] private AudioClip shootAudio;
    [SerializeField] private AudioClip shootFailAudio;

    private HashSet<Collider2D> fluffsInPointer = new HashSet<Collider2D>(); // Lista de pelusas que almacenamos cuando estan en el area del puntero

    void OnTriggerEnter2D(Collider2D enemy)
    {
        // Detectamos la pelusa que ha entrado en el area del puntero
        fluffsInPointer.Add(enemy);
    }

    void OnTriggerExit2D(Collider2D outEnemy)
    {
        // Detectamos la pelusa que se está saliendo del area del puntero para eliminarla de la lista
        fluffsInPointer.Remove(outEnemy);
    }

    void Update()
    {
        // Primero comprobamos si se ha pulsado alguna de las teclas con las que se puede eliminar una pelusa
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L))
        {
            // Si se ha pulsado alguna de las teclas, probamos si hay alguna pelusa en el área para ese color
            if (fluffsInPointer != null && fluffsInPointer.Count > 0)
            {
                Collider2D find = null;

                if (Input.GetKeyDown(KeyCode.H))
                    find = fluffsInPointer.FirstOrDefault(f => f.GetComponent<Fluff>().type == EnemyType.Red);
                else if (Input.GetKeyDown(KeyCode.J))
                    find = fluffsInPointer.FirstOrDefault(f => f.GetComponent<Fluff>().type == EnemyType.Yellow);
                else if (Input.GetKeyDown(KeyCode.K))
                    find = fluffsInPointer.FirstOrDefault(f => f.GetComponent<Fluff>().type == EnemyType.Green);
                else if (Input.GetKeyDown(KeyCode.L))
                    find = fluffsInPointer.FirstOrDefault(f => f.GetComponent<Fluff>().type == EnemyType.Blue);

                if (find != null)
                {
                    fluffsInPointer.Remove(find);
                    Destroy(find.gameObject);
                    ScoreManager.Instance.AddOrSubstractPoints(10, find.GetComponent<Fluff>().type);
                    SoundManager.Instance.PlaySound(shootAudio);
                }
                else
                {
                    // Si se ha pulsado una tecla pero no es la correcta, se penaliza con puntuación
                    ScoreManager.Instance.AddOrSubstractPoints(-5); // Puedes cambiar el tipo de enemigo según tu lógica
                    SoundManager.Instance.PlaySound(shootFailAudio);
                }

                UpdateTexts();
            }
        }
    }

    /// <summary>
    /// Actualiza textos de puntuación
    /// </summary>
    private void UpdateTexts()
    {
        if (totalScoreText != null)
            totalScoreText.text = Mathf.FloorToInt(ScoreManager.Instance.totalScore).ToString();
        if (lastScoreText != null)
        {
            lastScoreText.color = ScoreManager.Instance.lastScore > 0 ? Color.green : Color.red;
            lastScoreText.text = (ScoreManager.Instance.lastScore >= 0 ? "(+" : "(") + ScoreManager.Instance.lastScore + ")";
        }
    }
}