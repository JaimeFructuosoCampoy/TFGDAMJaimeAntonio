using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 5.0f;
    public bool isMovingRight = true;
    private Vector2 startPos;
    private Rigidbody2D rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = isMovingRight ? 1.0f : -1.0f;
        Vector2 move = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(rb.position + move);

        float movedDistance = Vector2.Distance(rb.position, startPos);
        if (movedDistance >= distance)
        {
            isMovingRight = !isMovingRight;
            startPos = rb.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión con: " + collision.collider.name); // Agregá esto
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("¡Colisión con pared!");
            isMovingRight = !isMovingRight;
            startPos = rb.position;
        }
    }
}

