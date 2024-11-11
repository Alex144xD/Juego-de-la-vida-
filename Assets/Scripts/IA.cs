using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IA : MonoBehaviour
{
    public float speed = 3f;
    private Transform target;
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        // Verifica si el temporizador ha terminado antes de permitir el movimiento
        if (gameController.CanMove())
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Point").transform;
            }

            if (target != null)
            {
                // Mueve la IA hacia el punto
                Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                // Restringe el movimiento dentro de los límites
                newPosition.x = Mathf.Clamp(newPosition.x, gameController.minBounds.x, gameController.maxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, gameController.minBounds.y, gameController.maxBounds.y);

                transform.position = newPosition;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            gameController.DeclareWinner("La IA");
        }
    }
}