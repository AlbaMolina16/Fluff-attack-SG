using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script que maneja la carga de escenas
/// </summary>
public class SceneLoader : MonoBehaviour
{
    #region OLD. Carga de escenas sin animación ni pantalla de carga. Cambio brusco de escena y freezy

    /// <summary>
    /// Carga la escena por su índice en el Build Settings
    /// </summary>
    /// <param name="index">número de índice de la escena en el Build Settings</param>
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    #endregion
    public void LoadSceneAsyncByIndex(int index)
    {
        StartCoroutine(LoadSceneCoroutine(index));
    }

    IEnumerator LoadSceneCoroutine(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}