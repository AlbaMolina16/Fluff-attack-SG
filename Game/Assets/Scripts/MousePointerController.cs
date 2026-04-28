using UnityEngine;

public class MousePointerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Obtiene la posición del mouse en pantalla
        Vector3 mousePosition = Input.mousePosition;
        // Convierte la posición a coordenadas del mundo
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Establece la posición del sprite (z debe ser 0 para 2D)
        mousePosition.z = 0f;
        transform.position = mousePosition;
    }
}