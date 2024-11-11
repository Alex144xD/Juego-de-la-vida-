using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject player;
    public GameObject ai;

    // Límites de movimiento
    public Vector2 minBounds = new Vector2(-8f, -4f);
    public Vector2 maxBounds = new Vector2(8f, 4f);

    private GameObject currentPoint;
    public bool gameEnded = false;

    // Temporizador de inicio
    public float startDelay = 55f;
    private float timer;

    void Start()
    {
        timer = startDelay;
        GenerateRandomPoint();
    }

    void Update()
    {
        // Reduce el temporizador
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool CanMove()
    {
        // Permite el movimiento solo si el temporizador ha terminado
        return timer <= 0 && !gameEnded;
    }

    void GenerateRandomPoint()
    {
        if (currentPoint != null)
        {
            Destroy(currentPoint);
        }

        // Genera un punto aleatorio dentro de los límites especificados
        Vector2 randomPosition = new Vector2(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y)
        );
        currentPoint = Instantiate(pointPrefab, randomPosition, Quaternion.identity);
    }

    public void DeclareWinner(string winner)
    {
        if (!gameEnded)
        {
            gameEnded = true;
            if (winner == "La IA")
            {
                Debug.Log("Has perdido");
            }
            else if (winner == "El jugador")
            {
                Debug.Log("Has ganado");
            }
        }
    }
}