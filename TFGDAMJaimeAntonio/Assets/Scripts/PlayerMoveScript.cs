using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private bool CanJump = false;
    private Rigidbody2D Rb2D;
    private float MovementSpeed = 3.5f;
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
        if (Input.GetKey("right"))
            Rb2D.velocity = Vector2.right * MovementSpeed;
        else if (Input.GetKey("left"))
            Rb2D.velocity = Vector2.left * MovementSpeed;
        else
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
    }

    private void CheckJump()
    {
        if (gameObject.transform.position.y <= 0)
        {
            CanJump = true;
        }
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey("up") && CanJump && gameObject.transform.position.y < 10))
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 8f * Time.deltaTime);
        }

    }
}