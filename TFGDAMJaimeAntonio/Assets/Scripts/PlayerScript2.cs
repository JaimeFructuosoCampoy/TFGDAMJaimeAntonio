using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerScript2 : MonoBehaviour
{
    private Rigidbody2D Rb2D;
    private SpriteRenderer Sprite;

    private float MovementSpeed = 3.5f;
    public float JumpSpeed = 5f;

    private bool IsGrounded = false;
    private bool IsTouchingWall = false;
    private bool IsTouchingLeftWall = false;
    private bool IsTouchingRightWall = false;

    private int Coins = 0;
    public TMP_Text CoinsText;

    public Collider2D GroundCheck;
    public Collider2D WallCheckLeft;
    public Collider2D WallCheckRight;

    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        gameObject.SetActive(!GlobalData.GameOver);
        CheckMovement();
        CheckJump();
        ApplyWallSlide();
    }

    private void CheckMovement()
    {
        float moveX = 0;

        if (Input.GetKey("right") || Input.GetKey("d"))
            moveX = 1;
        else if (Input.GetKey("left") || Input.GetKey("a"))
            moveX = -1;

        // Si estás en el aire y tocando una pared en esa dirección, cancelamos movimiento
        if (!IsGrounded && IsTouchingWall)
        {
            if ((moveX > 0 && IsTouchingRightWall) || (moveX < 0 && IsTouchingLeftWall))
            {
                moveX = 0;
            }
        }

        Rb2D.velocity = new Vector2(moveX * MovementSpeed, Rb2D.velocity.y);

        if (moveX > 0)
            Sprite.flipX = true;
        else if (moveX < 0)
            Sprite.flipX = false;
    }

    private void CheckJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("up") || Input.GetKeyDown("w")))
        {
            if (IsGrounded)
            {
                Rb2D.velocity = new Vector2(Rb2D.velocity.x, JumpSpeed);
            }
            else if (IsTouchingLeftWall)
            {
                Rb2D.velocity = new Vector2(5f, JumpSpeed); // Salta hacia la derecha
            }
            else if (IsTouchingRightWall)
            {
                Rb2D.velocity = new Vector2(-5f, JumpSpeed); // Salta hacia la izquierda
            }
        }
    }

    private void ApplyWallSlide()
    {
        //Si está tocando pared, no está en el suelo, y está cayendo
        if (IsTouchingWall && !IsGrounded && Rb2D.velocity.y < 0)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, -1.5f); // Ajusta velocidad vertical de caída
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Ground" || tag == "Platform")
            IsGrounded = true;

        if (tag == "Wall")
            IsTouchingWall = true;

        if (tag == "Enemy")
            GlobalData.GameOver = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Ground" || tag == "Platform")
            IsGrounded = false;

        if (tag == "Wall")
            IsTouchingWall = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        // Detectamos suelo desde el collider GroundCheck
        if (collision == GroundCheck)
        {
            if (tag == "Ground" || tag == "Platform")
                IsGrounded = true;
        }

        // Detectamos pared izquierda
        if (collision == WallCheckLeft)
        {
            if (tag == "Wall")
                IsTouchingLeftWall = true;
        }

        // Detectamos pared derecha
        if (collision == WallCheckRight)
        {
            if (tag == "Wall")
                IsTouchingRightWall = true;
        }

        // Detectamos moneda y enemigo en el trigger general del jugador
        if (tag == "Coin")
        {
            Destroy(collision.gameObject);
            Coins++;
            CoinsText.SetText(Coins.ToString());
        }

        if (tag == "Enemy")
        {
            GlobalData.GameOver = true;
        }
    }

        private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (collision == GroundCheck)
        {
            if (tag == "Ground" || tag == "Platform")
                IsGrounded = false;
        }

        if (collision == WallCheckLeft)
        {
            if (tag == "Wall")
                IsTouchingLeftWall = false;
        }

        if (collision == WallCheckRight)
        {
            if (tag == "Wall")
                IsTouchingRightWall = false;
        }
    }

    public int GetCoins
    {
        get { return Coins; }
    }
}
