using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text GameOverText;
    private int Points = 0;
    public TMP_Text CountText;
    private float tiempoActual;

    //LLuvia
    public CloudSpawn cloudSpawn;
    public float startRainTime;
    private bool rainActive = false;

    //Pausa
    private bool isPaused = false;

    //Lluvia meteoritos 
    public GameObject meteoroPrefab;
    public float intervalMeteoro = 1.5f;
    public float alturaY = 10f;
    public float xMin = -8f;
    public float xMax = 8f;
    public float startMeteoroTime;
    private bool meteoroActive = false;



    void Start()
    {
        tiempoActual = 0f;
        Time.timeScale = 1f; //Aseguramos que el tiempo no se pausa al incio
    }

    // Update is called once per frame
    void Update()
    {
        //Presionamos P para pausar el juego
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchPause();
        }

        //Si el juego esta pausado o si el juego termina, no hacemos nada
        if (GlobalData.GameOver || isPaused)
        {
            if (GlobalData.GameOver)
            {
                ShowGameOver();
            }

            return; //No seguimos si esta pausado o si el juego termina
        }
        else
        {
            UpdateCount();

            //Miramos si es el momento de activar la lluvia (si es asi lo activamos)
            if (!rainActive)
            {
                tiempoActual += Time.deltaTime;

                if (tiempoActual >= startRainTime)
                {
                    cloudSpawn.StartRain(); //Activa lluvia
                    rainActive = true;
                }
            }

            //Miramos si es el momento de activar la lluvia de meteoritos (si es asi lo activamos)
            if (!meteoroActive)
            {
                tiempoActual += Time.deltaTime;
                if (tiempoActual >= startMeteoroTime)
                {
                    StartCoroutine(SpawnMeteoro());
                    meteoroActive = true;
                }
            }
        }


    }

    private void ShowGameOver()
    {
        GameOverText.gameObject.SetActive(true);
    }

    private void UpdateCount()
    {
        Points++;
        CountText.SetText(Points.ToString());
    }

    /// <summary>
    /// Cambia el estado de pausa del juego
    /// Alternamos el estado de isPaused y ajustamos el tiempo de juego
    ///     dependiendo del estado de isPaused
    /// </summary>
    private void SwitchPause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// Genera meteoritos en la parte superior de la pantalla
    /// </summary>
    /// <returns>
    ///  Se repite cada intervalMeteoro segundos
    /// </returns>
    IEnumerator SpawnMeteoro()
    {
        while (true)
        {
            float randomX = Random.Range(xMin, xMax);
            Vector2 spawnPos = new Vector2(randomX, alturaY);

            Instantiate(meteoroPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(intervalMeteoro);
        }
    }
}
