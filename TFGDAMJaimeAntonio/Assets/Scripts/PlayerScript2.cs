using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript2 : MonoBehaviour
{
    private bool CanJump;
    private Rigidbody2D Rb2D;
    private SpriteRenderer Sprite;
    private float MovementSpeed = 3.5f;
    public float JumpSpeed = 5f;
    private int Coins = 0;
    public TMP_Text CoinsText;

    // Referencias a los botones móviles
    public GameObject MobileControls;
    public Button ButtonLeft;
    public Button ButtonRight;
    public Button ButtonJump;

    //Estados de los botones
    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private bool isJumpPressed = false;

    // Variables de estado
    private bool touchingLeftWall = false;
    private bool touchingRightWall = false;
    private bool isGrounded = false;

    public float wallSlideSpeed = -0.5f; // Velocidad de deslizamiento por la pared

    // Start is called before the first frame update  
    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();

        // Mostrar controles móviles solo en Android
        if (MobileControls != null)
        MobileControls.SetActive(Application.platform == RuntimePlatform.Android);
    }

    // Métodos para EventTrigger
    public void OnLeftDown() { isLeftPressed = true; }
    public void OnLeftUp() { isLeftPressed = false; }
    public void OnRightDown() { isRightPressed = true; }
    public void OnRightUp() { isRightPressed = false; }
    public void OnJumpDown() { isJumpPressed = true; }
    public void OnJumpUp() { isJumpPressed = false; }

    // Update is called once per frame  
    void FixedUpdate()
    {
        gameObject.SetActive(!GlobalData.GameOver);

        // DEBUG: Imprimir estado actual
        Debug.Log($"Frame: touchingLeft={touchingLeftWall}, touchingRight={touchingRightWall}, velocityY={Rb2D.velocity.y}");

        CheckMovement();
        CheckJump();

        if (touchingLeftWall || touchingRightWall) // Sin más condiciones
        {
            Debug.Log("ENTRANDO A WALL SLIDE - Aplicando velocidad constante");
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, wallSlideSpeed);
            Debug.Log($"DESPUÉS de wall slide: velocityY={Rb2D.velocity.y}");
        }
    }

    private void CheckMovement()
    {
        float move = 0;

        bool leftInput = Input.GetKey("left") || Input.GetKey("a") || isLeftPressed;
        bool rightInput = Input.GetKey("right") || Input.GetKey("d") || isRightPressed;

        // Si pulsa izquierda y está tocando la pared izquierda, anula movimiento
        if (leftInput && touchingLeftWall)
        {
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
            Sprite.flipX = false;
            return;
        }
        // Si pulsa derecha y está tocando la pared derecha, anula movimiento
        if (rightInput && touchingRightWall)
        {
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
            Sprite.flipX = true;
            return;
        }

        if (rightInput)
        {
            move = 1;
            Sprite.flipX = true;
        }
        else if (leftInput)
        {
            move = -1;
            Sprite.flipX = false;
        }

        Rb2D.velocity = new Vector2(move * MovementSpeed, Rb2D.velocity.y);
    }


    private void CheckJump()
    {
        bool jumpInput = Input.GetKey(KeyCode.Space) || Input.GetKey("up") || Input.GetKey("w") || isJumpPressed;

        // Solo puede saltar si está en el suelo (CanJump) y NO tocando ninguna pared lateral
        if (jumpInput && CanJump && isGrounded && !touchingLeftWall && !touchingRightWall)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, JumpSpeed);
            isJumpPressed = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Ground":
            case "Platform":
                CanJump = true;
                break;
            case "Wall":
                // No activar CanJump aquí
                break;
            case "Enemy":
                GlobalData.GameOver = true;
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        switch (tag)
        {
            case "Ground":
            case "Platform":
                CanJump = false;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        Debug.Log("Trigger ENTER: " + tag + " - " + collision.gameObject.name);
        switch (tag)
        {
            case "WallCheckLeft":
                touchingLeftWall = true;
                break;
            case "WallCheckRight":
                touchingRightWall = true;
                break;
            case "Ground":
                isGrounded = true;
                break;
            case "Coin":
                Destroy(collision.gameObject);
                Coins++;
                CoinsText.SetText(Coins.ToString());
                break;
            case "Enemy":
                GlobalData.GameOver = true;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        Debug.Log("Trigger exit: " + tag);
        switch (tag)
        {
            case "WallCheckLeft":
                touchingLeftWall = false;
                break;
            case "WallCheckRight":
                touchingRightWall = false;
                break;
            case "Ground":
                isGrounded = false;
                break;
        }
    }
    public int GetCoins
    {
        get { return Coins; }
    }


}
