using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class RandomizeTsunamiBodyScript : MonoBehaviour
{
    private SpriteRenderer Sprite;
    private float FlipInterval = 0.5f;

    void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(FlipSprites());
    }

    IEnumerator FlipSprites()
    {
        while (true)
        {
            int randomNumber = Random.Range(0, 4);
            switch (randomNumber)
            {
                case 0:
                    Sprite.flipX = true;
                    break;

                case 1:
                    Sprite.flipY = true;
                    break;

                case 2:
                    Sprite.flipX = false;
                    break;

                case 3:
                    Sprite.flipY = false;
                    break;
            }
            yield return new WaitForSeconds(FlipInterval);
        }
    }
}
