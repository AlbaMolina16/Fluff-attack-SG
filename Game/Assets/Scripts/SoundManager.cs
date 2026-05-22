using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton que se encarga de reproducir los sonidos del juegos.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource; // Componente de audio para reproducir sonidos

    private void Awake()
    {
        // Si no existe la instancia de SoundManager, la asignamos y hacemos que no se destruya entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Reproduce un clip de audio en la escena.
    /// </summary>
    /// <param name="clip">Archivo de audio</param>
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}