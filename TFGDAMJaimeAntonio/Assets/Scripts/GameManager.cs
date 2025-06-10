using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Referencias a los componentes de texto de TMP para mostrar el tiempo
    public TMP_Text SecondText;
    public TMP_Text MinuteText;
    public TMP_Text HourText;
    private TimerScript Timer;

    //Referencias puntuacion del juego
    private int PointCount = 0;
    public TMP_Text PointCountText;
    private int Level = 1;


    //Variables de control del juego

    public GameObject TsunamiLimit;
    public GameObject[] CataclysmsObjects;
    private Dictionary<int, bool> IsRandomUbicationCataclysm;
    public GameObject TsunamiStart;
    public GameObject Player;
    private float TsunamiSpeed = 0.25f;
    private float TimeUntilNewCataclysm;
    private bool CataclysmIsNotRandomUbicationEnded;
    private Dictionary<int, bool> IsRandomUbicationEnemy;
    public GameObject[] EnemyObjects;
    private float TimeUntilNewEnemy;

    //Manager de IA
    public QuestionHandler questionHandler;

    //PAUSA
    public Button ButtonPause;
    private bool IsPaused = false;
    public GameObject PauseObject;
    public Button ButtonContinue;

    //GameOver
    public GameObject GameOverObject;
    public Button ButtonPlayAgain;
    private bool gameOverShown = false;

    //Panel negro de fondo para los popups
    public GameObject backBlack;

    //Cataclysmos posibles
    private enum Cataclysms
    {
        CLOUD_RAIN, METEORITE, TSUNAMI, SPIKES, BLACK_HOLE
    }

    //Enmigos posibles
    private enum Enemies
    {
        BASIC, FLYING, SHOOT
    }
    

    /// <summary>
    /// Método Awake se llama cuando la instancia del script está siendo cargada.
    /// Inicializa el componente TimerScript.
    /// </summary>
    private void Awake()
    {
    
        GameObject timerObject = new GameObject("TimerScript");
        Timer = timerObject.AddComponent<TimerScript>();
        Timer.SetTextReferences(SecondText, MinuteText, HourText);
    }

    /// <summary>
    /// Método Start se llama antes de la primera actualización del frame.
    /// Inicializa el estado del juego, configura los listeners de los botones y congela el tiempo para la pregunta inicial.
    /// </summary>
    void Start()
    {
        Time.timeScale = 0f; //Congelamos para la pregunta

        questionHandler.onPopupClosed += StartGame;

        CataclysmIsNotRandomUbicationEnded = true;
        InitializeKeyValueCataclysmUbication();
        InitializeKeyValueEnemyUbication();

        if (ButtonPause != null)
            ButtonPause.onClick.AddListener(SwitchPause);
        if (ButtonContinue != null)
            ButtonContinue.onClick.AddListener(SwitchPause);
        if (ButtonPlayAgain != null)
            ButtonPlayAgain.onClick.AddListener(HideGameOverMenuAndRestart);

        //Solo aseguramos la escala
        if (GameOverObject != null)
            GameOverObject.transform.localScale = Vector3.zero;
    }

    
    /// <summary>
    /// Método Update se llama una vez por frame.
    /// Gestiona el estado de pausa y game over, y actualiza los puntos si el juego está en curso.
    /// </summary>
    void Update()
    {
        CheckPause();
        if (GlobalData.GameOver || IsPaused)
        {
            if (GlobalData.GameOver)
            {
                if (!gameOverShown)
                {
                    ShowGameOverMenu();
                    gameOverShown = true;
                }
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
            //Reinicia el flag cuando el juego se reanuda
            gameOverShown = false;

            if (Timer.CheckUpdateCounts())
            {
                UpdatePointCount();
                Timer.VerifyDataIntegrity();
                Timer.UpdateTime();
            }
        }
    }


    /// <summary>
    /// Muestra el menú de Game Over con una animación.
    /// </summary>
    private void ShowGameOverMenu()
    {
        backBlack.SetActive(true);
        LeanTween.cancel(GameOverObject); //Cancela cualquier animación previa pendiente  
        GameOverObject.transform.localScale = Vector3.zero;
        GameOverObject.SetActive(true);
        LeanTween.scale(GameOverObject, new Vector3(1f, 1f, 1), 0.5f) 
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
        Debug.Log("Mostrando GameOver");
    }

    /// <summary>
    /// Oculta el menú de Game Over, desactiva el fondo negro y reinicia la escena actual.
    /// </summary>
    private void HideGameOverMenuAndRestart()
    {
        BackgroundQuit();
        GlobalData.GameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Incrementa el contador de puntos y actualiza el texto en la interfaz de usuario.
    /// </summary>
    private void UpdatePointCount()
    {
        PointCount++;
        PointCountText.SetText(PointCount.ToString());
    }

    /// <summary>
    /// Cambia el estado de pausa del juego. Muestra u oculta el menú de pausa y ajusta la escala de tiempo.
    /// </summary>
    private void SwitchPause()
    {
        IsPaused = !IsPaused;
        if (IsPaused)
        {
            ShowPauseMenu();
            Time.timeScale = 0f;
        }
        else
        {
            HidePauseMenu();
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Comprueba si se ha presionado la tecla de pausa (P) y si el juego no ha terminado para cambiar el estado de pausa.
    /// </summary>
    private void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.P) && !GlobalData.GameOver)
        {
            SwitchPause();
        }
    }

    /// <summary>
    /// Corrutina que espera un tiempo aleatorio antes de iniciar un nuevo cataclismo.
    /// </summary>
    IEnumerator WaitUntilCataclysm()
    {
        while (true)
        {
            TimeUntilNewCataclysm = UnityEngine.Random.Range(5f, 30f);
            Debug.Log("Se esperarn " + TimeUntilNewCataclysm + " segundos para ejecutar el siguiente cataclismo");
            yield return new WaitForSeconds(TimeUntilNewCataclysm);
            StartCoroutine(SelectAndStartCataclysm());
        }
    }
    /// <summary>
    /// Corrutina que espera un tiempo aleatorio antes de generar un nuevo enemigo.
    /// </summary>
    IEnumerator WaitUntilEnemy()
    {
        while (true)
        {
            TimeUntilNewEnemy = UnityEngine.Random.Range(3f, 15f);
            Debug.Log("Se esperarn " + TimeUntilNewEnemy + " segundos para generar un nuevo enemigo");
            yield return new WaitForSeconds(TimeUntilNewEnemy);
            StartCoroutine(SelectAndSpawnEnemy());
        }
    }

    /// <summary>
    /// Corrutina que selecciona un cataclismo aleatorio, determina su ubicación, lo instancia y programa su destrucción.
    /// </summary>
    IEnumerator SelectAndStartCataclysm()
    {
        int cataclysm = UnityEngine.Random.Range(0, 3);
        if (CataclysmIsNotRandomUbicationEnded)
        {
            Debug.Log("Se ha seleccionado el cataclismo " + (Cataclysms)cataclysm);
            Vector3? v = SelectCataclysmUbication(cataclysm);
            if (v.HasValue)
            {
                GameObject instance = Instantiate(CataclysmsObjects[cataclysm], (Vector3)v, Quaternion.identity);
                int secondsUntilDestroy = UnityEngine.Random.Range(0, 30); //Hay que convertir estos valores en atributos
                Debug.Log("El cataclismo " + (Cataclysms)cataclysm + " ser destruido en " + secondsUntilDestroy);
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

    /// <summary>
    /// Corrutina que selecciona un tipo de enemigo aleatorio, determina su posición de aparición, lo instancia y programa su destrucción.
    /// </summary>
    IEnumerator SelectAndSpawnEnemy()
    {
        int enemyType = UnityEngine.Random.Range(0, EnemyObjects.Length);
        Debug.Log("Se ha seleccionado el enemigo " + (Enemies)enemyType);
        Vector3? spawnPosition = SelectEnemyUbication(enemyType);
        if (spawnPosition.HasValue)
        {
            GameObject instance = Instantiate(EnemyObjects[enemyType], (Vector3)spawnPosition, Quaternion.identity);
            int secondsUntilDestroy = UnityEngine.Random.Range(10, 60);
            Debug.Log("El enemigo " + (Enemies)enemyType + " ser destruido en " + secondsUntilDestroy + " segundos");
            yield return new WaitForSeconds(secondsUntilDestroy);
            Destroy(instance);
        }
    }

    /// <summary>
    /// Selecciona una ubicación para el cataclismo. Devuelve una posición aleatoria si el cataclismo lo permite, o null si tiene una ubicación fija.
    /// </summary>
    /// <param name="cataclysm">El índice del tipo de cataclismo.</param>
    /// <returns>Un Vector3 con la posición o null.</returns>
    private Vector3? SelectCataclysmUbication(int cataclysm)
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

    /// <summary>
    /// Selecciona una ubicación para el enemigo. Devuelve una posición aleatoria si el enemigo lo permite, o null si tiene una ubicación fija.
    /// </summary>
    /// <param name="enemyType">El índice del tipo de enemigo.</param>
    /// <returns>Un Vector3 con la posición o null.</returns>
    private Vector3? SelectEnemyUbication(int enemyType)
    {
        if (IsRandomUbicationEnemy[enemyType])
        {
            float y = UnityEngine.Random.Range(-1f, 3f);
            float x = UnityEngine.Random.Range(-4f, 4f);
            Vector3 vector = new Vector3(x, y, 0f);
            return vector;
        }
        return null;
    }

    /// <summary>
    /// Inicializa el diccionario que determina si la ubicación de cada cataclismo es aleatoria o predefinida.
    /// </summary>
    private void InitializeKeyValueCataclysmUbication()
    {
        IsRandomUbicationCataclysm = new Dictionary<int, bool>();
        IsRandomUbicationCataclysm.Add(0, true);
        IsRandomUbicationCataclysm.Add(1, true);
        IsRandomUbicationCataclysm.Add(2, false);
    }

    /// <summary>
    /// Inicializa el diccionario que determina si la ubicación de aparición de cada enemigo es aleatoria o predefinida.
    /// </summary>
    private void InitializeKeyValueEnemyUbication()
    {
        IsRandomUbicationEnemy = new Dictionary<int, bool>();
        IsRandomUbicationEnemy.Add(0, true); // BASIC  
        IsRandomUbicationEnemy.Add(1, true); // FLYING  
    }

    /// <summary>
    /// Corrutina que ejecuta el cataclismo del tsunami, controlando su movimiento hacia un límite y su posterior retroceso.
    /// </summary>
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
        
        Debug.Log("Se ha llegado a la posicin del tsunami");

        yield return new WaitForSeconds(5f);

        Debug.Log("Se ha comenzado a mover el tsunami hacia la posicin inicial");

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

    /// <summary>
    /// Mtodo que se llama cuando el popup de la IA se cierra.
    /// Reanuda el tiempo y comienza las corrutinas del juego para cataclismos y enemigos.
    /// </summary>
    private void StartGame()
    {
        //Reanudar el tiempo y comenzar las corrutinas del juego
        Time.timeScale = 1f;
        StartCoroutine(WaitUntilCataclysm());
        StartCoroutine(WaitUntilEnemy());
    }

    /// <summary>
    /// Aumenta la escala de gravedad del Rigidbody2D del jugador.
    /// </summary>
    public void addGravity() 
    { 
        Player.GetComponent<Rigidbody2D>().gravityScale = 1.2f;
    }

    /// <summary>
    /// Reduce la escala de gravedad del Rigidbody2D del jugador.
    /// </summary>
    public void removeGravity()
    {
        Player.GetComponent<Rigidbody2D>().gravityScale = 0.8f;
    }

    /// <summary>
    /// Muestra el menú de pausa con una animación.
    /// </summary>
    private void ShowPauseMenu()
    {
        backBlack.SetActive(true);
        PauseObject.SetActive(true);
        LeanTween.scale(PauseObject, new Vector3(1f, 1f, 1), 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
    }

    /// <summary>
    /// Oculta el menú de pausa con una animación y desactiva el objeto del menú de pausa.
    /// </summary>
    private void HidePauseMenu()
    {
        LeanTween.scale(PauseObject, new Vector3(0, 0, 0), 0.5f)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                PauseObject.SetActive(false);
                BackgroundQuit();
            });
    }

    /// <summary>
    /// Desactiva el panel de fondo negro utilizado para los popups.
    /// </summary>
    public void BackgroundQuit()
    {
        backBlack.SetActive(false);
    }

    /// <summary>
    /// Restablece el tiempo a la normalidad, actualiza los puntos del jugador y carga la escena del menú principal.
    /// </summary>
    public void ReturnToMenu()
    {
        //Volver a poner el tiempo en normal
        Time.timeScale = 1f;
        GlobalData.GameOver = false;
        //Implementar lgica de actualizacin de Monedas y Puntos aqu
        PlayerLoggedIn.Points += PointCount;
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Comprueba el objeto equipado por el jugador y aplica la lógica correspondiente.
    /// </summary>
    public void CheckEquipedObject()
    {
        string equipedObject = PlayerLoggedIn.ItemEquiped.name;
        switch (equipedObject)
        {
            case "Mete-Helmet":
                //Equipar casco
                break;
            case "Metal Umbrella":
                //Equipar paraguas
                break;
            case "Bouncing boots":
                //Equipar botas
                break;
        }
    }
}