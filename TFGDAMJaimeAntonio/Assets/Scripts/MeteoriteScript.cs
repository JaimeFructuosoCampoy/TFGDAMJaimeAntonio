using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteScript : MonoBehaviour
{
    public float DelayExplosion = 1f;
    public float TimeUntilDestroy = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            gameObject.GetComponent<Animator>().SetBool("ExplodeParam", true);
            StartCoroutine(GlobalFunctions.DestroyObjectAfterSeconds(TimeUntilDestroy, gameObject));
        }
    }
}