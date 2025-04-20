using System.Collections;
using UnityEngine;

public class CloudScript : MonoBehaviour
{

    public float Speed = 1.0f;
    public float Distance = 5.0f;
    public bool IsMovingRight = true;
    private Vector2 StartPosition;
    private Rigidbody2D RigidBody;

    void Start()
    {
        StartPosition = transform.position;
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = IsMovingRight ? 1.0f : -1.0f;
        Vector2 move = new Vector2(direction * Speed * Time.fixedDeltaTime, 0f);
        RigidBody.MovePosition(RigidBody.position + move);

        float movedDistance = Vector2.Distance(RigidBody.position, StartPosition);
        if (movedDistance >= Distance)
        {
            IsMovingRight = !IsMovingRight;
            StartPosition = RigidBody.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            IsMovingRight = !IsMovingRight;
            StartPosition = RigidBody.position;
        }
    }
   
}
