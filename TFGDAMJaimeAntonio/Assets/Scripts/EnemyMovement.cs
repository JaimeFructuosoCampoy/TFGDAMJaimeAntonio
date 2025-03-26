using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{
    private float moveDirection;
    public float speed = 2f;
    private bool movingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ComprobarDireccion();
    }

    //Detecta colisiones con otros objetos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        movingRight = !movingRight;
    }

    private void ComprobarDireccion()
    {
        if (movingRight)
            moveDirection = 1f; //Movimiento hacia la derecha
        else
            moveDirection = -1f; //Movimiento hacia la izquierda
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y); //Mantenemos la velocidad en Y sin alterarla
    }
}