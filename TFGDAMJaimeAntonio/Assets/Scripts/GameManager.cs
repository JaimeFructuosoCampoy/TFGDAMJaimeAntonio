using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text SecondText;
    public TMP_Text MinuteText;
    public TMP_Text HourText;
    public TMP_Text GameOverText;
    public TMP_Text PauseText;
    private TimerScript Timer;
    private int PointCount = 0;
    public TMP_Text PointCountText;
    private int Level = 1;
    private bool IsPaused = false;
    public GameObject[] CataclysmsObjects;
    private enum Cataclysms
    {
        CLOUD_RAIN, METEORITE, TSUNAMI
    }

    private void Awake()
    {
        GameObject timerObject = new GameObject("TimerScript");
        Timer = timerObject.AddComponent<TimerScript>();
        Timer.SetTextReferences(SecondText, MinuteText, HourText);
    }

    void Start()
    {
        StartCoroutine(WaitUntilCataclysm());
    }

    // Update is called once per frame
    void Update()
    {
        CheckPause();
        if (GlobalData.GameOver || IsPaused)
        {
            if (GlobalData.GameOver)
            {
                ShowGameOver();
                if (Input.GetKeyDown(KeyCode.R))
                {
                    GlobalData.GameOver = false;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            return;
        }
        else
        {
            if (Timer.CheckUpdateCounts())
            {
                UpdatePointCount();
                Timer.VerifyDataIntegrity();
                Timer.UpdateTime();
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

    private void SwitchPause()
    {
        IsPaused = !IsPaused;
        PauseText.gameObject.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    private void VerifyDataIntegrity()
    {

    }

    private void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.P) && !GlobalData.GameOver)
        {
            SwitchPause();
        }
    }

    IEnumerator WaitUntilCataclysm()
    {
        while (true)
        {
            int time = Random.Range(5, 30);
            Debug.Log("Se esperarán " + time + " segundos para ejecutar el siguiente cataclismo");
            yield return new WaitForSeconds(time);
            StartCoroutine(SelectAndStartCataclysm());
        }
    }

    IEnumerator SelectAndStartCataclysm()
    {
        int cataclysm = Random.Range(0, 2);
        Debug.Log("Se ha seleccionado el cataclismo " + (Cataclysms)cataclysm);
        Vector3 vector = SelectUbication(cataclysm);
        GameObject instance = Instantiate(CataclysmsObjects[cataclysm], vector, Quaternion.identity);
        int secondsUntilDestroy = Random.Range(0, 30); //Hay que convertir estos valores en atributos
        Debug.Log("El cataclismo " + (Cataclysms)cataclysm + " será destruido en " + secondsUntilDestroy);
        yield return new WaitForSeconds(secondsUntilDestroy);
        Destroy(instance);
    }

    private Vector3 SelectUbication(int cataclysm)
    {
        float y = Random.Range(-1f, 3f);
        float x = 0f;
        if (cataclysm != 2)
        {
            x = Random.Range(-4f, 4f);
        }
        Vector3 vector = new Vector3(x, y, 0f);
        return vector;
    }
}
