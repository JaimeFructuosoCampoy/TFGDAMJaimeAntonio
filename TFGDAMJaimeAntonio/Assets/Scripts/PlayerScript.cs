using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private bool CanJump;
    private Rigidbody2D Rb2D;
    private SpriteRenderer Sprite;
    private float MovementSpeed = 3.5f;
    public float JumpSpeed = 5f;
    public int Coins = 0;
    public TMP_Text CoinsText;

    // Referencias a los botones m�viles
    public GameObject MobileControls;
    public Button ButtonLeft;
    public Button ButtonRight;
    public Button ButtonJump;

    //Estados de los botones
    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private bool isJumpPressed = false;

    // Start is called before the first frame update  
    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();

        // Mostrar controles m�viles solo en Android
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
        CheckMovement();
        CheckJump();
    }

    /// <summary>
    /// Verifica el movimiento del jugador basado en la entrada del teclado o los botones móviles.
    /// Actualiza la velocidad del Rigidbody2D en consecuencia.
    /// </summary>
    private void CheckMovement()
    {
        float move = 0;

        // Teclado
        if (Input.GetKey("right") || Input.GetKey("d") || isRightPressed)
        {
            move = 1;
            Sprite.flipX = true;
        }
        else if (Input.GetKey("left") || Input.GetKey("a") || isLeftPressed)
        {
            move = -1;
            Sprite.flipX = false;
        }
        Rb2D.velocity = new Vector2(move * MovementSpeed, Rb2D.velocity.y);
    }

    /// <summary>
    /// Verifica si el jugador puede saltar y aplica la velocidad de salto si es posible.
    /// El salto se activa con la tecla de espacio, flecha arriba, 'w' o el botón de salto móvil.
    /// /// </summary>
    private void CheckJump()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey("up") || Input.GetKey("w") || isJumpPressed) && CanJump)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, JumpSpeed);
            isJumpPressed = false; // Para que solo salte una vez por toque
        }
    }

    /// <summary>
    /// Maneja las colisiones del jugador con otros objetos.
    /// Detecta colisiones con el suelo, paredes, plataformas, enemigos y meteoritos.
    /// Si colisiona con un enemigo o meteorito sin el casco adecuado, termina el juego.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionGameobjectTag = collision.gameObject.tag;
        switch (collisionGameobjectTag)
        {
            case "Ground":
            case "Wall":
            case "Platform":
                //var directionsAndWays = GlobalFunctions.DetectDirectionAndWay(collision);  
                //var directionAndWay = directionsAndWays[0];  
                //if (!directionAndWay.Item1 && !directionAndWay.Item2) // Si la colisi�n es por arriba  
                //    CanJump = false;  
                //else if (!directionAndWay.Item1 || directionAndWay.Item2) // Si la colisi�n es por abajo  
                //    CanJump = true;  
                CanJump = true;
                break;
            case "Enemy":
                GlobalData.GameOver = true;
                break;
            case "Meteorite":
                if (PlayerLoggedIn.ItemEquiped.name != "Mete-Helmet")
                    GlobalData.GameOver = true;
                break;
        }
    }

    /// <summary>
    /// Maneja la salida de colisiones del jugador con otros objetos.
    /// Desactiva la capacidad de salto cuando el jugador sale de una colisión con el suelo, paredes o plataformas.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        string collisionGameobjectTag = collision.gameObject.tag;
        switch (collisionGameobjectTag)
        {
            case "Ground":
            case "Wall":
            case "Platform":
                CanJump = false;
                break;
        }
    }

    /// <summary>
    /// Maneja las colisiones del jugador con objetos que tienen un collider trigger.
    /// Detecta colisiones con monedas y enemigos.
    /// Si colisiona con una moneda, la destruye y aumenta el contador de monedas.
    /// Si colisiona con un enemigo, termina el juego.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string triggerGameobjectTag = collision.gameObject.tag;
        switch (triggerGameobjectTag)
        {
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

    /// <summary>
    /// Propiedad para obtener el número de monedas del jugador.
    /// </summary>
    public int GetCoins
    {
        get { return Coins; }
    }


}
