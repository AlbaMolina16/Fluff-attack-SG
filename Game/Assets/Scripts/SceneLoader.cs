using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script que maneja la carga de escenas
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public void LoadSceneAsyncByIndex(int index)
    {
        StartCoroutine(LoadSceneCoroutine(index));
    }

    /// <summary>
    /// Carga la escena de juego en función de si el nivel es adaptativo o no.
    /// </summary>
    public void LoadGameSceneAsyncByDifficulty()
    {
        if (UserSession.Instance.UserDifficulty == null)
        {
            Debug.LogError("No se ha seleccionado una dificultad. No se puede cargar la escena de juego.");
            return;
        }
        else
        {
            if (UserSession.Instance.UserDifficulty.name == "autoadaptative")
            {
                StartCoroutine(LoadSceneCoroutine(3));
            }
            else
            {
                StartCoroutine(LoadSceneCoroutine(1));
            }
        }
    }

    /// <summary>
    /// Corrutina para carga la escena sin freezy
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator LoadSceneCoroutine(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}