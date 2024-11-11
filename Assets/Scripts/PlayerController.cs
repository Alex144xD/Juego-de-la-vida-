using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        // Permite el movimiento solo si el temporizador ha terminado
        if (gameController.CanMove())
        {
            // Movimiento usando teclas de flecha
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(horizontal, vertical) * speed * Time.deltaTime;

            // Calcula la nueva posición con límites
            Vector2 newPosition = (Vector2)transform.position + movement;
            newPosition.x = Mathf.Clamp(newPosition.x, gameController.minBounds.x, gameController.maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, gameController.minBounds.y, gameController.maxBounds.y);

            transform.position = newPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            gameController.DeclareWinner("El jugador");
        }
    }
}