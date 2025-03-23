using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D Rb2D;
    private float Speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("right"))
            Rb2D.velocity = Vector2.right * Speed;
        else if (Input.GetKey("left"))
            Rb2D.velocity = Vector2.left * Speed;
    }
}
