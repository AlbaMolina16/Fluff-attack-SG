using UnityEngine;

/// <summary>
/// Script que gestiona la rotación del spinner de carga.
/// </summary>
public class SpinnerRotate : MonoBehaviour
{
    /// <summary>
    /// Velocidad de rotación del spinner en grados por segundo
    /// </summary>
    public float speed = 180f;

    /// <summary>
    /// Rota el spinner cada frame en función de la velocidad y el tiempo transcurrido para asegurar una rotación suave e independiente del framerate.
    /// </summary>
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}