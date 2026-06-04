using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script que se va a encarga de controlar el movimiento de las pelusas en caso de que lo tengan
/// </summary>
public class MovementController : MonoBehaviour
{
    // Qué necesitamos? 
    // 1. El tipo de movimiento, en nuestro caso según bbdd podría ser: ninguno, lineal, zigzag o errático
    // De momento sólo implementado el lineal y el zigzag
    // 2. Velocidad de movimiento mínima y máxima.
    // 3. La dirección del movimiento
    private DifficultyMovement movementType; // Tipo de movimiento a ejecutar
    private float speed;
    private Vector2 direction; // Dirección del movimiento
    private bool hasMovement = false;

    #region zigzag momvement
    private float zigzagTimer;
    private float zigzagInterval = 3f;
    #endregion

    /// <summary>
    /// Actualizará el movimiento del gameObject de la pelusa en caso de que se la haya asignado un tipo de movimiento
    /// </summary>
    void Update()
    {
        Vector3 pos = transform.position;

        if (hasMovement)
        {
            if (movementType.name == "zigzag")
            {
                zigzagTimer += Time.deltaTime;

                if (zigzagTimer >= zigzagInterval)
                {
                    // invertir eje vertical para crear zigzag
                    direction.y *= -1;
                    zigzagTimer = 0f;
                }
            }

            // Realiza movimiento
            pos += (Vector3)(direction * speed * Time.deltaTime);
            // Limitamos el rebote de la pelusa en los bordes de la pantalla
            // Horizontal
            if (pos.x < LimitAreaGame.InstanceMinPantalla.x || pos.x > LimitAreaGame.InstanceMaxPantalla.x)
            {
                direction.x *= -1; // Cambiamos el sentido de movimiento en x
                pos.x = Mathf.Clamp(pos.x, LimitAreaGame.InstanceMinPantalla.x, LimitAreaGame.InstanceMaxPantalla.x);
            }
            // Vertical
            if (pos.y < LimitAreaGame.InstanceMinPantalla.y || pos.y > LimitAreaGame.InstanceMaxPantalla.y)
            {
                direction.y *= -1; // Cambiamos el sentido de movimiento en x
                pos.y = Mathf.Clamp(pos.y, LimitAreaGame.InstanceMinPantalla.y, LimitAreaGame.InstanceMaxPantalla.y);
            }

            transform.position = pos;
        }
        else
        {
            return; // Pelusa fija
        }
    }

    /// <summary>
    /// Inicializa los parámetros del movimiento
    /// </summary>
    /// <param name="movement"></param>
    public void InitMovevement(DifficultyMovement movement)
    {
        movementType = movement;

        // Comprobamos si hay movimiento
        if (movementType != null && movementType.name != "none")
        {
            hasMovement = true;
            speed = UserSession.Instance.UserDifficulty != null ? UserSession.Instance.UserDifficulty.enemySpeed : 0f;

            switch (movementType.name)
            {
                case "lineal":
                    direction = Random.value < 0.5f ? Vector2.right :
                        (Random.value < 0.5f ? Vector2.left :
                             (Random.value < 0.5f ? Vector2.up : Vector2.down));
                    break;
                case "zigzag":
                    direction = new Vector2(1, 1).normalized; // inicial
                    break;
                default:
                    break;
            }
        }
        else
        {
            hasMovement = false;
        }
    }
}