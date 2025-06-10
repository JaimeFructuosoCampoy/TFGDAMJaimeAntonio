using UnityEngine;

public class Gout : MonoBehaviour
{
    public float Speed = 1.0f;
    public bool IsCloudGrandSon = false;
    void Update()
    {
        transform.Translate(Vector2.down * Speed * Time.deltaTime);
    }

    /// <summary>
    /// Metodo para detectar colisiones de la gota con otros objetos.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionName = collision.collider.tag;
        switch (collisionName)
        {
            case "Ground":
            case "Wall":
            case "Enemy":
                if (collision.collider.transform.parent != null && collision.collider.transform.parent.parent != null)
                {
                    if (transform.parent.parent != collision.collider.transform.parent.parent)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
                break;

            case "Platform":
                if (transform.parent.parent != collision.collider.transform)
                    Destroy(gameObject);
                break;

            case "Player":
                if (PlayerLoggedIn.ItemEquiped.name != "Metal Umbrella")
                {
                    GlobalData.GameOver = true;
                }
                else
                {
                    if (IsCloudGrandSon)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
        }
    }

}