using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{
    private float MoveDirection;
    public float Speed = 2f;
    private bool MovingRight = true;
    private Rigidbody2D RigidBody;
    private SpriteRenderer Sprite;
    void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ComprobarDireccion();
    }

    /// <summary>
    /// Metodo para detectar colisiones con el muro y cambiar la dirección del movimiento.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
            MovingRight = !MovingRight;
    }

    /// <summary>
    /// Metodo para comprobar la dirección del movimiento y actualizar la velocidad del Rigidbody.
    /// </summary>
    private void ComprobarDireccion()
    {
        if (MovingRight)
        {
            Sprite.flipX = true;
            MoveDirection = 1f;
        }
        else
        {
            Sprite.flipX = false;
            MoveDirection = -1f;
        }
        RigidBody.velocity = new Vector2(MoveDirection * Speed, RigidBody.velocity.y);
    }
}