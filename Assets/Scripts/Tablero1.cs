using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class Tablero1 : MonoBehaviour
{
    [SerializeField] private Tilemap EstadoActual;
    [SerializeField] private Tilemap SiguenteEstado;
    [SerializeField] private Tile vivo;
    [SerializeField] private Tile muerto;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;
    public int Numregla;

    // Para de limitar el área de la simulación.
    [SerializeField] private int LimiteX;
    [SerializeField] private int LimiteY;
    [SerializeField] private TMP_InputField inputFileX;
    [SerializeField] private TMP_InputField inputFieldY; 

    private HashSet<Vector3Int> CeldasVivas;
    private HashSet<Vector3Int> CeldasVerificar;
    public int population { get; private set; }
    public int iterations { get; private set; }
    public float time { get; private set; }
    void Update()
    {
        UpdateLimte();
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = EstadoActual.WorldToCell(mouseWorldPos);
            if (Mathf.Abs(cellPosition.x) <= LimiteX && Mathf.Abs(cellPosition.y) <= LimiteY) //checa los limites al dibujar.
            {
                EstadoActual.SetTile(cellPosition, vivo);
                CeldasVivas.Add(cellPosition);
            }
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
        var newAliveCells = new HashSet<Vector3Int>();  // Nuevo conjunto para las celdas vivas
        CeldasVerificar.Clear();

        // Evalua solo la fila actual de celdas vivas y sus vecinos
        foreach (Vector3Int cell in CeldasVivas)
        {
            Vector3Int leftNeighbor = cell + new Vector3Int(-1, 0, 0);
            Vector3Int rightNeighbor = cell + new Vector3Int(1, 0, 0);

            // Verifica que los vecinos estén dentro de los límites
            if (Mathf.Abs(leftNeighbor.x) <= LimiteX)
            {
                CeldasVerificar.Add(leftNeighbor);
            }
            if (Mathf.Abs(rightNeighbor.x) <= LimiteX)
            {
                CeldasVerificar.Add(rightNeighbor);
            }

            // Revisa la celda actual
            if (Mathf.Abs(cell.x) <= LimiteX)
            {
                CeldasVerificar.Add(cell);
            }
        }

        // Evalua el estado de las celdas para la siguiente generación
        foreach (Vector3Int cell in CeldasVerificar)
        {
            Vector3Int leftNeighbor = cell + new Vector3Int(-1, 0, 0);
            Vector3Int rightNeighbor = cell + new Vector3Int(1, 0, 0);

            // Implementa la regla 30
            bool newAliveState = Regla(IsVivo(leftNeighbor), IsVivo(cell), IsVivo(rightNeighbor));

            // Cambia el estado de la celda en el nivel inferior
            Vector3Int lowerCell = cell + new Vector3Int(0, -1, 0);
            if (newAliveState)
            {
                SiguenteEstado.SetTile(lowerCell, vivo);
                newAliveCells.Add(lowerCell);
            }
            else
            {
                SiguenteEstado.SetTile(lowerCell, vivo);
            }
        }

        // Actualiza las celdas vivas para la siguiente iteración
        CeldasVivas = newAliveCells;

        // Intercambiamos los Tilemaps
        Tilemap temp = EstadoActual;
        EstadoActual = SiguenteEstado;
        SiguenteEstado = temp;
    }

    private bool Regla(bool IzquierdaVivo, bool estadoVivo, bool DerechaVivo)
    {
        // regla
        int Est = 0;
        int EstI = 0;
        int EstC = 0;
        int EstD = 0;
        if (IzquierdaVivo)
             EstI = 4;
        else EstI = 0;
        if (estadoVivo)
             EstC = 2;
        else EstC = 0;
        if (DerechaVivo)
             EstD = 1;
        else EstD =0;
        Est = EstI+EstC+EstD;

        return((Numregla >> Est) & 1 ) ==1;
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

    public void UpdateLimte() // checa los limites 
    {
        if(int.TryParse(inputFileX.text, out int X))
        {
            LimiteX = X;
        }
        if(int.TryParse(inputFieldY.text, out int Y))
        {
            LimiteY = Y;
        }
    }

}
