using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoutSpawn : MonoBehaviour
{
    public GameObject Gout;
    private float Interval;
    private Coroutine ActiveRain;
    public bool IsRaining { get; private set; }

    private bool RainActive = false;
    private float CurrentTime;
    public float StartRainTime;
    public float MinIntervalValue = 0.5f;
    public float MaxIntervalValue = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        CurrentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!RainActive)
        {
            CurrentTime += Time.deltaTime;

            if (CurrentTime >= StartRainTime)
            {
                StartRain(); //Activa lluvia
                RainActive = true;
            }
        }
    }


    public void StartRain()
    {
        if (!IsRaining)
        {
            ActiveRain = StartCoroutine(SpawnGout());
            IsRaining = true;
        }
    }



    public void StopRain()
    {
        if (ActiveRain != null)
        {
            StopCoroutine(ActiveRain);
            ActiveRain = null;
            IsRaining = false;
        }
    }

    IEnumerator SpawnGout()
    {
        while (true)
        {
            Interval = Random.Range(MinIntervalValue, MaxIntervalValue);
            Vector3 spawnPosition = transform.position + new Vector3(0, -0.1f, 0);
            Instantiate(Gout, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(Interval);
        }
    }
}
