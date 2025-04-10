using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text GameOverText;
    private int Points = 0;
    public TMP_Text CountText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalData.GameOver == true)
            ShowGameOver();
        else
            UpdateCount();
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
