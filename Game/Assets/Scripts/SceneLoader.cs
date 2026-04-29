using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script que maneja la carga de escenas
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Carga la escena por su índice en el Build Settings
    /// </summary>
    /// <param name="index">número de índice de la escena en el Build Settings</param>
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}