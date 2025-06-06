using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public TMP_Text SecondText;
    public TMP_Text MinuteText;
    public TMP_Text HourText;
    public static TimerScript Instance { get; private set; }
    public float UpdateTimer = 0f;
    private uint Seconds = 0;
    private uint Minutes = 0;
    private uint Hours = 0;

    private uint LastSecondValue = 0;
    private uint LastMinuteValue = 0;
    private uint LastHourValue = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerifyDataIntegrity();
    }

    public bool CheckUpdateCounts()
    {
        UpdateTimer += Time.deltaTime;
        bool update = false;
        if (UpdateTimer >= 1f)
        {
            update = true;
            UpdateTimer = 0f;
        }
        return update;
    }

    public void SetTextReferences(TMP_Text secondText, TMP_Text minuteText, TMP_Text hourText)
    {
        SecondText = secondText;
        MinuteText = minuteText;
        HourText = hourText;
    }

    public void UpdateTime() 
    {
        Seconds++;

        if (Seconds >= 60)
        {
            Minutes++;
            Seconds = 0;
        }

        if (Minutes >= 60)
        {
            Hours++;
            Minutes = 0;
        }

        if (Hours >= 24) 
        {
            //Days++;
            Hours = 0;
        }

        LastSecondValue = Seconds;
        LastMinuteValue = Minutes;
        LastHourValue = Hours;

        SecondText.SetText(Seconds.ToString());
        MinuteText.SetText(Minutes.ToString());
        HourText.SetText(Hours.ToString());
    }

    public void VerifyDataIntegrity()
    {
        if (LastSecondValue != Seconds || LastMinuteValue != Minutes || LastHourValue != Hours)
        {
            Debug.LogError($"Detección de manipulación! " +
                         $"Tiempo actual: {Hours}:{Minutes}:{Seconds} | " +
                         $"Último válido: {LastHourValue}:{LastMinuteValue}:{LastSecondValue}");
            Application.Quit();
        }
    }
}
