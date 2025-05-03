using System;
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
    public GameObject TsunamiLimit;
    public GameObject[] CataclysmsObjects;
    private Dictionary<int, bool> IsRandomUbicationCataclysm;
    public GameObject TsunamiStart;
    public GameObject Player;
    private float TsunamiSpeed = 0.25f;
    private float TimeUntilNewCataclysm;
    private bool CataclysmIsNotRandomUbicationEnded;
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
        CataclysmIsNotRandomUbicationEnded = true;
        InitializeKeyValueUbication();
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
            TimeUntilNewCataclysm = UnityEngine.Random.Range(5f, 30f);
            Debug.Log("Se esperarán " + TimeUntilNewCataclysm + " segundos para ejecutar el siguiente cataclismo");
            yield return new WaitForSeconds(TimeUntilNewCataclysm);
            StartCoroutine(SelectAndStartCataclysm());
        }
    }

    IEnumerator SelectAndStartCataclysm()
    {
        int cataclysm = UnityEngine.Random.Range(0, 3);
        if (CataclysmIsNotRandomUbicationEnded)
        {
            Debug.Log("Se ha seleccionado el cataclismo " + (Cataclysms)cataclysm);
            Vector3? v = SelectUbication(cataclysm);
            if (v.HasValue)
            {
                GameObject instance = Instantiate(CataclysmsObjects[cataclysm], (Vector3)v, Quaternion.identity);
                int secondsUntilDestroy = UnityEngine.Random.Range(0, 30); //Hay que convertir estos valores en atributos
                Debug.Log("El cataclismo " + (Cataclysms)cataclysm + " será destruido en " + secondsUntilDestroy);
                yield return new WaitForSeconds(secondsUntilDestroy);
                Destroy(instance);
            }
            else
            {
                switch (cataclysm)
                {
                    case 2:
                        StartCoroutine(ExecuteTsunamiCataclysm());
                        break;
                }
            }
        }
    }

    private Vector3? SelectUbication(int cataclysm)
    {
        if (IsRandomUbicationCataclysm[cataclysm])
        {
            float y = UnityEngine.Random.Range(-1f, 3f);
            float x = UnityEngine.Random.Range(-4f, 4f);
            Vector3 vector = new Vector3(x, y, 0f);
            return vector;
        }
        return null;
    }

    private void InitializeKeyValueUbication()
    {
        IsRandomUbicationCataclysm = new Dictionary<int, bool>();
        IsRandomUbicationCataclysm.Add(0, true);
        IsRandomUbicationCataclysm.Add(1, true);
        IsRandomUbicationCataclysm.Add(2, false);
    }

    IEnumerator ExecuteTsunamiCataclysm()
    {
        CataclysmIsNotRandomUbicationEnded = false;
        TsunamiLimit.transform.parent = null;
        Vector3 tsunamiLimitPosition = TsunamiLimit.transform.position;
        Transform tsunamiTransform = CataclysmsObjects[2].transform;

        Vector3 tsunamiStartPosition = TsunamiStart.transform.position;

        while (tsunamiTransform.position.y != tsunamiLimitPosition.y)
        {
            tsunamiTransform.position = Vector3.MoveTowards(
                tsunamiTransform.position,
                new Vector3(tsunamiTransform.position.x, tsunamiLimitPosition.y, tsunamiTransform.position.z),
                TsunamiSpeed * Time.deltaTime
            );
            Debug.Log(tsunamiTransform.position.y - tsunamiLimitPosition.y);
            yield return new WaitForEndOfFrame(); // espera al siguiente frame
        }
        
        Debug.Log("Se ha llegado a la posición del tsunami");

        yield return new WaitForSeconds(5f);

        Debug.Log("Se ha comenzado a mover el tsunami hacia la posición inicial");

        while (tsunamiTransform.position.y != tsunamiStartPosition.y)
        {
            tsunamiTransform.position = Vector3.MoveTowards(
                tsunamiTransform.position,
                tsunamiStartPosition,
                TsunamiSpeed * Time.deltaTime
            );
            Debug.Log(tsunamiTransform.position.y - tsunamiStartPosition.y);
            yield return new WaitForEndOfFrame(); // espera al siguiente frame
        }
        
        TsunamiLimit.transform.parent = Player.transform;
        TsunamiLimit.transform.position = new Vector3(0, Player.transform.position.y + 1f, 0);
        CataclysmIsNotRandomUbicationEnded = true;
    }

}