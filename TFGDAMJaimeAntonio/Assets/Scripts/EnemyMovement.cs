using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{
    private float MoveDirection;
    public float Speed = 2f;
    private bool MovingRight = true;
    private Rigidbody2D RigidBody;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ComprobarDireccion();
    }

    //Detecta colisiones con otros objetos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        MovingRight = !MovingRight;
    }

    private void ComprobarDireccion()
    {
        if (MovingRight)
            MoveDirection = 1f; //Movimiento hacia la derecha
        else
            MoveDirection = -1f; //Movimiento hacia la izquierda
        RigidBody.velocity = new Vector2(MoveDirection * Speed, RigidBody.velocity.y); //Mantenemos la velocidad en Y sin alterarla
    }
}