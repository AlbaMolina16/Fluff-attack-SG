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

    void Start()
    {
        // Initialize the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Prevent the player from rotating
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
    private void LimitPosition()
    {
        // Obtiene la posición del puntero
        Vector3 pos = transform.position;

        // Limitar posición del puntero
        pos.x = Mathf.Clamp(pos.x, LimitAreaGame.InstanceMinPantalla.x, LimitAreaGame.InstanceMaxPantalla.x);
        pos.y = Mathf.Clamp(pos.y, LimitAreaGame.InstanceMinPantalla.y, LimitAreaGame.InstanceMaxPantalla.y);

        transform.position = pos;
    }

    void FixedUpdate()
    {
        // Apply movement to the player in FixedUpdate for physics consistency
        rb.velocity = movement * speed;
    }
}
