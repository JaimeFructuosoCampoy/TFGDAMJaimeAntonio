using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private bool CanJump;
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
        gameObject.SetActive(!GlobalData.GameOver);
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
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey("up") || Input.GetKey("w")) && CanJump)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, JumpSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionGameobjectTag = collision.gameObject.tag;
        switch (collisionGameobjectTag)
        {
            case "Ground":
                CanJump = true;
                break;
            case "Enemy":
                GlobalData.GameOver = true;
                break;
        }
            CanJump = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            CanJump = false;
        }
    }
}