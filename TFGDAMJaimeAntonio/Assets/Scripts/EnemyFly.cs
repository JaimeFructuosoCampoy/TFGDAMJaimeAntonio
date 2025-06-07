using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float Speed = 1.5f;
    public float Distance = 4.0f; //Distancia hasta que cambia de direccion
    private bool IsMovingRight = true;
    private Vector2 StartPos;
    private SpriteRenderer EnemySprite;
    private int test;
    void Start()
    {
        StartPos = transform.position;
        EnemySprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float direction = IsMovingRight ? 1f : -1f;
        float move = Speed * Time.deltaTime * direction;

        transform.Translate(move, 0f, 0f);

        float movedDistance = Vector2.Distance(transform.position, StartPos);

        if (movedDistance >= Distance)
        {
            IsMovingRight = !IsMovingRight;
            StartPos = transform.position;

            //Volteo el sprite horizontalmente cuando cambia de direccion
            if (EnemySprite != null)
            {
                EnemySprite.flipX = !EnemySprite.flipX;
            }
        }
    }
}
