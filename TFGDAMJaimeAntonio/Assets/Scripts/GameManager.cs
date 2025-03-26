using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public TMP_Text GameOverText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalData.GameOver == true)
            ShowGameOver();
    }

    private void ShowGameOver()
    {
        GameOverText.gameObject.SetActive(true);
    }
}
