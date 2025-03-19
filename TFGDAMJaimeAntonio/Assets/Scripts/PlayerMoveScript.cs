using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private bool CanJump = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
        CheckJump();
    }

    private void CheckMovement()
    {
        if (Input.GetKey("left"))
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + -3f * Time.deltaTime, gameObject.transform.position.y);
        else if (Input.GetKey("right"))
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + 3f * Time.deltaTime, gameObject.transform.position.y);
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
