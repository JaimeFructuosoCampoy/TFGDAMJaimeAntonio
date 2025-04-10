using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 4.0f; //Distancia hasta que cambia de direccion
    private bool isMovingRight = true;
    private Vector2 startPos;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        float direction = isMovingRight ? 1f : -1f;
        float move = speed * Time.deltaTime * direction;

        transform.Translate(move, 0f, 0f);

        float movedDistance = Vector2.Distance(transform.position, startPos);

        if (movedDistance >= distance)
        {
            isMovingRight = !isMovingRight;
            startPos = transform.position;

            //Volteo el sprite horizontalmente cuando cambia de direccion
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
    }
}
