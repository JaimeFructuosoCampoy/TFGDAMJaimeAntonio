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

    //Detecta colisiones con otros objetos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
            MovingRight = !MovingRight;
    }

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