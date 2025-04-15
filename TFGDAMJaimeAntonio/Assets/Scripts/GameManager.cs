using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text GameOverText;
    private int Points = 0;
    public TMP_Text CountText;

    public CloudSpawn cloudSpawn;
    public float startRainTime;
    private float tiempoActual;
    private bool rainActive = false;
    void Start()
    {
        tiempoActual = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalData.GameOver == true)
        {
            ShowGameOver();
        }
        else
        {
            UpdateCount();
            if (!rainActive)
            {
                tiempoActual += Time.deltaTime;

                if (tiempoActual >= startRainTime)
                {
                    cloudSpawn.StartRain(); //Activa lluvia
                    rainActive = true;
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
}
