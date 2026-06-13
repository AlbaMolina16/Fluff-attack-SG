using UnityEngine;

/// <summary>
/// Singleton que almacena la posición donde se eliminan pelusas representandolo sobre una cuadrícula
/// Permite detectar las zonas donde ha habido menos interacción para forzar el spawn hacia ellas
/// y forzar la movilidad de muñeca durante la rehabilitación.
/// </summary>
public class HeatmapTracker : MonoBehaviour
{
    public static HeatmapTracker Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("HeatmapTracker");
                _instance = go.AddComponent<HeatmapTracker>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    private static HeatmapTracker _instance;

    /// Matriz de filas por columnas, por ejemplo 4*4 
    /// [] [] [] []
    /// [] [] [] []
    /// [] [] [] []
    /// [] [] [] []
    private int[,] _grid;
    private int _columns;
    private int _rows;
    private Vector2 _min;
    private Vector2 _max;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa (o reinicia) la cuadricula de calor.
    /// Debe llamarse al inicio de cada partida adaptativa.
    /// </summary>
    public void Initialize(int cols, int rows)
    {
        // Nos aseguramos de que al menos se configure una cuadricula de 1*1
        _columns = Mathf.Max(1, cols);
        _rows = Mathf.Max(1, rows);
        _min = LimitAreaGame.InstanceMinPantalla;
        _max = LimitAreaGame.InstanceMaxPantalla;
        _grid = new int[_columns, _rows];
    }

    /// <summary>
    /// Registra un acierto del jugador en la posicion de la pantalla donse se ha hecho
    /// </summary>
    public void RegisterHit(Vector2 worldPos)
    {
        if (_grid == null) return;

        float rangeX = _max.x - _min.x;
        float rangeY = _max.y - _min.y;
        if (rangeX <= 0f || rangeY <= 0f) return;

        int col = Mathf.Clamp(Mathf.FloorToInt((worldPos.x - _min.x) / rangeX * _columns), 0, _columns - 1);
        int row = Mathf.Clamp(Mathf.FloorToInt((worldPos.y - _min.y) / rangeY * _rows), 0, _rows - 1);
        _grid[col, row]++;
    }

    /// <summary>
    /// Devuelve el centro en coordenadas del mundo de la celda con menos aciertos registrados.
    /// Si hay varias celdas empatadas en minimo, elige una al azar entre ellas para evitar
    /// patrones predecibles.
    /// </summary>
    public Vector2 GetColdZoneCenter()
    {
        if (_grid == null) return Vector2.zero;

        // Encontrar el minimo de hits
        int minHits = int.MaxValue;
        for (int c = 0; c < _columns; c++)
            for (int r = 0; r < _rows; r++)
                if (_grid[c, r] < minHits) minHits = _grid[c, r];

        // Recoger todas las celdas con ese minimo
        var coldCells = new System.Collections.Generic.List<(int col, int row)>();
        for (int c = 0; c < _columns; c++)
            for (int r = 0; r < _rows; r++)
                if (_grid[c, r] == minHits) coldCells.Add((c, r));

        // Elegir una al azar
        var chosen = coldCells[Random.Range(0, coldCells.Count)];

        float cellW = (_max.x - _min.x) / _columns;
        float cellH = (_max.y - _min.y) / _rows;
        float x = _min.x + (chosen.col + 0.5f) * cellW;
        float y = _min.y + (chosen.row + 0.5f) * cellH;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Limpia todos los contadores de la cuadricula.
    /// </summary>
    public void Clear()
    {
        if (_grid != null)
            System.Array.Clear(_grid, 0, _grid.Length);
    }
}