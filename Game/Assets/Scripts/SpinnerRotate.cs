using UnityEngine;

/// <summary>
/// Script que gestiona la rotación del spinner de carga.
/// </summary>
public class SpinnerRotate : MonoBehaviour
{
    public float speed = 180f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}