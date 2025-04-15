using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoroScript : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float delayExplosion = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, delayExplosion); //Destruye la explosión
            Destroy(gameObject);// Destruye el meteorito
        }
    }

}
