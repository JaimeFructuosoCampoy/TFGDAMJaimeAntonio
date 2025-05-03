using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFlipController : MonoBehaviour
{
    private List<SpriteRenderer> TsunamiBodiesSprites;
    private float flipInterval = 0.5f;

    void Awake()
    {
        TsunamiBodiesSprites = new List<SpriteRenderer>();
        SpriteRenderer[] allChildsSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in allChildsSprites)
        {
            if (sprite.gameObject.name.Contains("TsunamiBody"))
            {
                TsunamiBodiesSprites.Add(sprite);
            }
        }
        
    }

    private void Start()
    {
        StartCoroutine(FlipAllSprites());
    }

    IEnumerator FlipAllSprites()
    {
        while (true)
        {
            foreach (SpriteRenderer sprite in TsunamiBodiesSprites)
            {
                int randomNumber = Random.Range(0, 5);

                switch (randomNumber)
                {
                    case 0:
                        sprite.flipX = true;
                        break;

                    case 1:
                        sprite.flipY = true;
                        break;

                    case 2:
                        sprite.flipX = false;
                        break;

                    case 3:
                        sprite.flipY = false;
                        break;
                    case 4:
                        break;
                }
            }

            // Esperar antes de repetir
            yield return new WaitForSeconds(flipInterval);
        }
    }
}
