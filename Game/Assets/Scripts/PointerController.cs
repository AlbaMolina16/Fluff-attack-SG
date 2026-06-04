using System;
using UnityEngine;

/// <summary>
/// Este script controla el movimiento de la mirilla en el juego
/// </summary>
public class PointerController : MonoBehaviour
{
        
    [Header("Velocidad de movimiento del puntero")]
    // Public variables
    public float speed = 5f; // The speed at which the player moves

    // Private variables 
    private Rigidbody2D rb; // Reference to the Rigidbody2D component attached to the player
    private Vector2 movement; // Stores the direction of player movement
    private float radius;

    void Start()
    {
        // Initialize the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Prevent the player from rotating
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        radius = GetComponent<SpriteRenderer>().bounds.extents.x; // Obtenemos el radio del spriteRender del crosshair
    }

    void Update()
    {
        // Get player input from keyboard or controller
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Set movement direction based on input
        movement = new Vector2(horizontalInput, verticalInput);

        LimitPosition();
    }

    /// <summary>
    /// Función para limitar la posición del puntero, ya que al estar el Collider como Istrigger para la detección de pelusas,
    /// no estaba colisionando con las paredes que habían incluido. Así que hay que limitarlo en función de los límites de la pantalla.
    /// </summary>
    private void LimitPosition()
    {
        // Obtiene la posición del puntero
        Vector3 pos = transform.position;

        // Limitar posición del puntero
        transform.position = LimitAreaGame.SpriteLimit(pos, radius); // Ajusta el radio del puntero según sea necesario (en este caso, 0.5f)
    }

    void FixedUpdate()
    {
        // Apply movement to the player in FixedUpdate for physics consistency
        rb.velocity = movement * speed;
    }
}
