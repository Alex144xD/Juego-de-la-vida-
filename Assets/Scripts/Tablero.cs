using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tablero : MonoBehaviour
{
    [SerializeField] private Tilemap EstadoActual;
    [SerializeField] private Tilemap SiguienteEstado;
    [SerializeField] private Tile vivo; // arena 
    [SerializeField] private Tile muerto; // islas flotantes

    // Nuevas tiles para diferentes tipos de terreno
    [SerializeField] private Tile tierra;
    [SerializeField] private Tile agua;
    [SerializeField] private Tile bosque;

    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;
    [SerializeField] private int anchoMapa = 10;  // Ancho del mapa aleatorio
    [SerializeField] private int altoMapa = 10;   // Alto del mapa aleatorio

    private HashSet<Vector3Int> CeldasVivas;
    private HashSet<Vector3Int> CeldasVerificar;
    public int population { get; private set; }
    public int iterations { get; private set; }
    public float time { get; private set; }

    private void Awake()
    {
        CeldasVivas = new HashSet<Vector3Int>();
        CeldasVerificar = new HashSet<Vector3Int>();
    }

    private void Start()
    {
        GenerarMapaAleatorio();
        SetPattern(pattern);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = EstadoActual.WorldToCell(mouseWorldPos);
            EstadoActual.SetTile(cellPosition, vivo);
            CeldasVivas.Add(cellPosition);
        }
    }

    private void GenerarMapaAleatorio()
    {
        Clear();

        // Generar diferentes terrenos aleatoriamente
        for (int x = -anchoMapa / 2; x < anchoMapa / 2; x++)
        {
            for (int y = -altoMapa / 2; y < altoMapa / 2; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Tile tileAleatoria = ObtenerTileAleatoria();
                EstadoActual.SetTile(cellPosition, tileAleatoria);

                // Si la celda es de tipo "vivo", la añadimos a las celdas vivas
                if (tileAleatoria == vivo)
                {
                    CeldasVivas.Add(cellPosition);
                }
            }
        }
    }

    private Tile ObtenerTileAleatoria()
    {
        // Probabilidad de aparición de cada tipo de tile
        int randomValue = Random.Range(0, 100);

        if (randomValue < 40)      // 40% tierra
            return tierra;
        else if (randomValue < 70) // 30% agua
            return agua;
        else if (randomValue < 90) // 20% bosque
            return bosque;
        else                       // 10% celda "viva"
            return vivo;
    }

    private void SetPattern(Pattern pattern)
    {
        
        Vector2Int center = pattern.GetCenter();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - center);
            EstadoActual.SetTile(cell, vivo);
            CeldasVivas.Add(cell);
        }
        population = CeldasVivas.Count;
    }

    private void Clear()
    {
        EstadoActual.ClearAllTiles();
        SiguienteEstado.ClearAllTiles();
        CeldasVivas.Clear();
        CeldasVerificar.Clear();
        population = 0;
        iterations = 0;
        time = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(Simulación());
    }

    private IEnumerator Simulación()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;
        while (enabled)
        {
            UpdateState();

            population = CeldasVivas.Count;
            iterations++;
            time += updateInterval;

            yield return interval;
        }
    }

    private void UpdateState()
    {
        foreach (Vector3Int cell in CeldasVivas)
        {
            if (Mathf.Abs(cell.x) > anchoMapa/2|| Mathf.Abs(cell.y) > altoMapa/2) // revisa que esté dentro del límite
            {
                continue;
            }
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    CeldasVerificar.Add(cell + new Vector3Int(x, y, 0));
                }
            }
        }

        foreach (Vector3Int cell in CeldasVerificar)
        {
            if (Mathf.Abs(cell.x) > anchoMapa/2 || Mathf.Abs(cell.y) > altoMapa/2) // revisa que esté dentro del límite
            {
                continue;
            }

            int neighbors = CountNeighbors(cell);
            bool Vivo = IsVivo(cell);

            if (!Vivo && neighbors == 3)
            {
                SiguienteEstado.SetTile(cell, vivo);
                CeldasVivas.Add(cell);
            }
            else if (Vivo && (neighbors < 2 || neighbors == 8))
            {
                SiguienteEstado.SetTile(cell, muerto);
                CeldasVivas.Remove(cell);
            }
            else if (Vivo && (neighbors > 4 && neighbors < 7))
            {
                SiguienteEstado.SetTile(cell, bosque);
                CeldasVivas.Add(cell);
            }
            else if (Vivo && (neighbors > 3 && neighbors < 5))
            {
                SiguienteEstado.SetTile(cell, tierra);
                CeldasVivas.Add(cell);
            }
            else if (Vivo && (neighbors > 6 && neighbors < 8))
            {
                SiguienteEstado.SetTile(cell, agua);
                CeldasVivas.Add(cell);
            }
            else
            {
                SiguienteEstado.SetTile(cell, EstadoActual.GetTile(cell));
            }
        }

        Tilemap temp = EstadoActual;
        EstadoActual = SiguienteEstado;
        SiguienteEstado = temp;
        SiguienteEstado.ClearAllTiles();
    }

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                if (x == 0 && y == 0)
                {
                    continue;
                }
                if (IsVivo(neighbor))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsVivo(Vector3Int cell)
    {
        return EstadoActual.GetTile(cell) == vivo;
    }

}

