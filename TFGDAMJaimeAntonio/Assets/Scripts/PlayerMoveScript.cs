using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private bool CanJump = false;
    private Rigidbody2D Rb2D;
    private float MovementSpeed = 3.5f;
    public float JumpSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckMovement();
        CheckJump();
    }

    private void CheckMovement()
    {
        if (Input.GetKey("right") || Input.GetKey("d"))
            Rb2D.velocity = new Vector2(MovementSpeed, Rb2D.velocity.y);
        else if (Input.GetKey("left") || Input.GetKey("a"))
            Rb2D.velocity = new Vector2(-MovementSpeed, Rb2D.velocity.y);
        else
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
    }

    private void CheckJump()
    {
        if (Rb2D.velocity.y <= 0)
        {
            CanJump = true;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey("up") || Input.GetKey("w")) && CanJump && gameObject.transform.position.y < 10)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, JumpSpeed);
        }

    }
}