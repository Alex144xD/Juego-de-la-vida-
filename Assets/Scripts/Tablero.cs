using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tablero : MonoBehaviour
{
    [SerializeField] private Tilemap EstadoActual;
    [SerializeField] private Tilemap SiguenteEstado;
    [SerializeField] private Tile vivo;
    [SerializeField] private Tile muerto;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;

    private HashSet<Vector3Int> CeldasVivas;
    private HashSet<Vector3Int> CeldasVerificar;
    public int population { get; private set; }
    public int iterations { get; private set; }
    public float time { get; private set; }
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
    private void Awake()
    {
        CeldasVivas = new HashSet<Vector3Int>();
        CeldasVerificar = new HashSet<Vector3Int>();
    }
    private void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Clear();

        Vector2Int center = pattern.GetCenter();

        for(int i = 0; i < pattern.cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int) (pattern.cells[i] - center);
            EstadoActual.SetTile(cell, vivo);
            CeldasVivas.Add(cell);
        }
        population = CeldasVivas.Count;
    }

    private void Clear ()
    {
       EstadoActual.ClearAllTiles();
       SiguenteEstado.ClearAllTiles();
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

            population=CeldasVivas.Count;
            iterations++;
            time += updateInterval;

            yield return interval;
        }
    }
    private void UpdateState()
    {
        foreach (Vector3Int cell in CeldasVivas)
        {

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    CeldasVerificar.Add(cell + new Vector3Int(x,y,0));
                }
            }
        }

        foreach(Vector3Int cell in CeldasVerificar)
        {
            int neighbors = CountNeighbors(cell);
            bool Vivo =  IsVivo(cell);

            if(!Vivo && neighbors == 3)
            {
                SiguenteEstado.SetTile(cell, vivo);
                CeldasVivas.Add(cell);
            }
            else if (Vivo && (neighbors < 2 || neighbors > 3))
            {
                SiguenteEstado.SetTile(cell, muerto);
                CeldasVivas.Remove(cell);
            }
            else
            {
                SiguenteEstado.SetTile(cell, EstadoActual.GetTile(cell));
            }
        }

        Tilemap temp = EstadoActual;
        EstadoActual = SiguenteEstado;
        SiguenteEstado = temp;
        SiguenteEstado.ClearAllTiles();
    }
    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;

        for(int x=-1; x<= 1; x++)
        {
            for(int y=-1; y<=1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x,y,0);
                if(x==0 && y == 0)
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
        return EstadoActual.GetTile(cell)== vivo;
    }

}
