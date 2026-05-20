using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Fluff;

/// <summary>
/// Este script se encarga de detectar si se está apuntando a un enemido y si se está pulsando la tecla indicada para eliminarlo.
/// También va añadiendo puntos por cada enemigo eliminado
/// </summary>
public class PointerShooter : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;
    private float points = 0f;
    // private Fluff actualEnemy;

    private HashSet<Collider2D> fluffsInPointer = new HashSet<Collider2D>(); // Lista de pelusas que almacenamos cuando estan en el area del puntero

    void OnTriggerEnter2D(Collider2D enemy)
    {
        // Detectamos la pelusa que ha entrado en el area del puntero
        // var fluff = enemy.GetComponent<Fluff>();
        fluffsInPointer.Add(enemy);
        // actualEnemy = enemy.GetComponent<Fluff>();
    }

    void OnTriggerExit2D(Collider2D outEnemy)
    {
        // Detectamos la pelusa que se está saliendo del area del puntero para eliminarla de la lista
        // var outFluff = outEnemy.GetComponent<Fluff>();
        fluffsInPointer.Remove(outEnemy);
        // actualEnemy = null;
    }

    void Update()
    {
        // Comprobamos si se está elimando una pelusa que se encuentra en el área de acción del puntero
        if (fluffsInPointer != null && fluffsInPointer.Count > 0)
        {
            List<Collider2D> toRemove = new List<Collider2D>();
            
            // Primero comprobamos si se ha pulsado la tecla correcta para eliminar la pelusa y la almacenamos
            foreach (Collider2D fluffCollider in fluffsInPointer)
            {
                Fluff fluff = fluffCollider.GetComponent<Fluff>();
                if ((Input.GetKeyDown(KeyCode.H) && fluff.type == EnemyType.Red)
                    || (Input.GetKeyDown(KeyCode.J) && fluff.type == EnemyType.Yellow)
                    || (Input.GetKeyDown(KeyCode.K) && fluff.type == EnemyType.Green)
                    || (Input.GetKeyDown(KeyCode.L) && fluff.type == EnemyType.Blue))
                {
                    toRemove.Add(fluffCollider);
                    // Destroy(fluff.gameObject);
                    points += 1;
                    pointsText.text = Mathf.FloorToInt(points).ToString();
                }
            }

            // Eliminamos de HasSet y testruimos el gameObject
            foreach (Collider2D fluffCollider in toRemove)
            {
                fluffsInPointer.Remove(fluffCollider);
                Destroy(fluffCollider.gameObject);
            }
        }
    }
}