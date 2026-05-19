using TMPro;
using UnityEngine;
using static Fluff;

/// <summary>
/// Este script se encarga de detectar si se está apuntando a un enemido y si se está pulsando la tecla indicada para eliminarlo.
/// También va añadiendo puntos por cada enemigo eliminado
/// </summary>
public class PointerShooter : MonoBehaviour
{
    public float detectionRadius = 0.5f;
    public LayerMask enemyLayer;

    [SerializeField] private TMP_Text pointsText;
    private float points = 0f;

    void Update()
    {
        // Detectar enemigo debajo de la mirilla
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);

        if (hit == null)
            return;

        Fluff enemy = hit.GetComponent<Fluff>();
        if (enemy == null)
            return;

        IsShooting(enemy);
    }

    private void IsShooting(Fluff enemy)
    {
        // Comprobar teclas
        if ((Input.GetKeyDown(KeyCode.H) && enemy.type == EnemyType.Red)
            || (Input.GetKeyDown(KeyCode.J) && enemy.type == EnemyType.Yellow)
            || (Input.GetKeyDown(KeyCode.K) && enemy.type == EnemyType.Green)
            || (Input.GetKeyDown(KeyCode.L) && enemy.type == EnemyType.Blue))
        {
            Destroy(enemy.gameObject);
            points += 1;
        }

        pointsText.text = Mathf.FloorToInt(points).ToString();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
