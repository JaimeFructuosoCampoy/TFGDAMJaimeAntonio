using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteScript : MonoBehaviour
{
    public float DelayExplosion = 1f;
    public float TimeUntilDestroy = 1f;

    /// <summary>
    /// Controla la colisión del meteorito con el suelo o el jugador.
    /// Si colisiona con el suelo, inicia la animación de explosión y destruye el objeto después de un tiempo.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            gameObject.GetComponent<Animator>().SetBool("ExplodeParam", true);
            StartCoroutine(GlobalFunctions.DestroyObjectAfterSeconds(TimeUntilDestroy, gameObject));
        }

        if (collision.collider.CompareTag("Player"))
        {
            if (PlayerLoggedIn.ItemEquiped.name != "Mete-Helmet")
            {
                GlobalData.GameOver = true;
            }
            else
            {
                gameObject.GetComponent<Animator>().SetBool("ExplodeParam", true);
                StartCoroutine(GlobalFunctions.DestroyObjectAfterSeconds(TimeUntilDestroy, gameObject));
            }
        }
    }


}