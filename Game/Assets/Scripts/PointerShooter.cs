using UnityEngine;
using static Fluff;

public class PointerShooter : MonoBehaviour
{
    public float detectionRadius = 0.5f;
    public LayerMask enemyLayer;

    void Update()
    {
        // Detectar enemigo debajo de la mirilla
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);

        if (hit == null)
            return;

        Fluff enemy = hit.GetComponent<Fluff>();
        if (enemy == null)
            return;

        // Comprobar teclas
        if (Input.GetKeyDown(KeyCode.H) && enemy.type == EnemyType.Red)
            Destroy(hit.gameObject);

        if (Input.GetKeyDown(KeyCode.J) && enemy.type == EnemyType.Yellow)
            Destroy(hit.gameObject);

        if (Input.GetKeyDown(KeyCode.K) && enemy.type == EnemyType.Green)
            Destroy(hit.gameObject);

        if (Input.GetKeyDown(KeyCode.L) && enemy.type == EnemyType.Blue)
            Destroy(hit.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
