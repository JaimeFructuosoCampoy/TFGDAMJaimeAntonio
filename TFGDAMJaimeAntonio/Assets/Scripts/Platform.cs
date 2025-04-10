using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Speed = 1.0f;
    public float Distance = 5.0f;
    public bool IsMovingRight = true;
    private Vector2 StartPos;
    private Rigidbody2D rb;

    void Start()
    {
        StartPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = IsMovingRight ? 1.0f : -1.0f;
        Vector2 move = new Vector2(direction * Speed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(rb.position + move);

        float movedDistance = Vector2.Distance(rb.position, StartPos);
        if (movedDistance >= Distance)
        {
            IsMovingRight = !IsMovingRight;
            StartPos = rb.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión con: " + collision.collider.name); // Agregá esto
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("¡Colisión con pared!");
            IsMovingRight = !IsMovingRight;
            StartPos = rb.position;
        }
    }
}

