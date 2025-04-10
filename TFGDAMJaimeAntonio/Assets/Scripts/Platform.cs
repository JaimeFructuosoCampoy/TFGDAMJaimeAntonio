using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 5.0f; //Distancia de movimiento
    private bool isMovingRight = true;
    private Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float direction;

        if (isMovingRight)
        {
            direction = 1.0f; //Mover a la derecha
        }
        else
        {
            direction = -1.0f; //Mover a la izquierda
        }
        float move = speed * Time.deltaTime * direction;

        transform.Translate(move, 0f, 0f);

        float movedDistace = Vector2.Distance(transform.position, startPos);

        if (movedDistace >= distance)
        {
            isMovingRight = !isMovingRight; //Cambia la direccion
            startPos = transform.position; //Actualizamos posicion
        }
    }
}
