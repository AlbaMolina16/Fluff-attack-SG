using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script que maneja la carga de escenas.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}