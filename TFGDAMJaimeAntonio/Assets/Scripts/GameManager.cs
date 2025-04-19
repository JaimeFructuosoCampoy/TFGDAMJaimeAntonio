using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text SecondText;
    public TMP_Text MinuteText;
    public TMP_Text HourText;
    public TMP_Text GameOverText;
    private TimerScript Timer;
    private int PointCount = 0;
    public TMP_Text PointCountText;
    private int Level = 1;

    private enum Cataclysms
    {
        CLOUD_RAIN, CLOUD_THUNDER, METEORITE, 
    }

    public CloudScript Cloud;
    public float StartRainTime;
    private float TiempoActual;
    private bool RainActive = false;

    private void Awake()
    {
        GameObject timerObject = new GameObject("TimerScript");
        Timer = timerObject.AddComponent<TimerScript>();
        Timer.SetTextReferences(SecondText, MinuteText, HourText);
    }

    void Start()
    {
        TiempoActual = 0f;
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
            if (Timer.CheckUpdateCounts() == true)
            {
                UpdatePointCount();
                Timer.VerifyDataIntegrity();
                Timer.UpdateTime();
            }

            if (!RainActive)
            {
                TiempoActual += Time.deltaTime;

                if (TiempoActual >= StartRainTime)
                {
                    Cloud.StartRain(); //Activa lluvia
                    RainActive = true;
                }
            }
        }


    }

    private void ShowGameOver()
    {
        GameOverText.gameObject.SetActive(true);
    }

    private void UpdatePointCount()
    {
        PointCount++;
        PointCountText.SetText(PointCount.ToString());
    }

    private void EvaluateRandomCataclysm()
    {
        int numeroTiempo = Random.Range(0, 6);

    }

    private void VerifyDataIntegrity()
    {

    }
}
