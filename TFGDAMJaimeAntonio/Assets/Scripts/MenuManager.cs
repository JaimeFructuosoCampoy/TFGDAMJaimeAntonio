using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMP_Text TitleText;
    public GameObject LoggedInOptions;
    public GameObject LoggedOutOptions;
    public GameObject PopUpLenguage;
    private bool isPopUpActive = false;
    public TMP_Text NameText;
    public TMP_Text PointText;
    public TMP_Text CoinText;

    public GameObject SettingsPopUp;
    public GameObject backBlack;

    private bool isSettingsPopUpActive = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        GlobalData.GameOver = false;
        if (SceneManager.GetActiveScene().name != "MenuScene")
        {
            Destroy(this);
        }

        AnimateTitle();
        SettingsPopUp.SetActive(false);
        backBlack.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        LoggedInOptions.SetActive(GlobalData.PlayerLoggedIn);
        LoggedOutOptions.SetActive(!GlobalData.PlayerLoggedIn);
        if (GlobalData.PlayerLoggedIn)
        {
            NameText.text = PlayerLoggedIn.FriendlyName;
            PointText.text = PlayerLoggedIn.Points.ToString();
            CoinText.text = PlayerLoggedIn.Coins.ToString();
        }
        
    }

    private void AnimateTitle()
    {
        LeanTween.rotate(TitleText.gameObject, new Vector3(0f, 0f, 2.5f), 1f)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.rotate(TitleText.gameObject, new Vector3(0f, 0f, -2.5f), 1f)
                .setIgnoreTimeScale(true) // <-- Y esto
                .setOnComplete(() =>
                {
                    AnimateTitle();
                });
            });
    }

    /// <summary>
    /// Metodo que hace de interruptor para el boton de lenguage
    /// </summary>
    public void TogglePopUPLenguage()
    {
        if (isPopUpActive)
        {
            ClosePopUp();
        }
        else
        {
            OpenPopUp();
        }
    }


    /// <summary>
    /// Metodo para abrir el popup de seleccion de idioma
    /// </summary>
    private void OpenPopUp()
    {
        PopUpLenguage.SetActive(true);

        PopUpLenguage.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(PopUpLenguage, new Vector3(1.7f, 1.7f, 1), 0.5f).setEaseOutBack();

        isPopUpActive = true;
    }

    /// <summary>
    /// Metodo para cerrar el popup de seleccion de idioma
    /// </summary>

    private void ClosePopUp()
    {
        LeanTween.scale(PopUpLenguage, new Vector3(0, 0, 0), 0.5f).setEaseInBack().setOnComplete(() =>
        {
            PopUpLenguage.SetActive(false);
        });

        isPopUpActive = false;
    }

    /// <summary>
    /// Método público para el botón de Ajustes y el botón de Volver.
    /// Abre o cierra el menú de ajustes.
    /// </summary>
    public void ToggleSettingsPopUp()
    {
        if (isSettingsPopUpActive)
        {
            HideSettingsPopUp();
        }
        else
        {
            ShowSettingsPopUp();
        }
    }

    public void ShowSettingsPopUp()
    {
        backBlack.SetActive(true);
        SettingsPopUp.SetActive(true);

        // Reseteamos la escala a 0 para que la animación siempre funcione bien
        SettingsPopUp.transform.localScale = Vector3.zero;

        LeanTween.scale(SettingsPopUp, Vector3.one, 0.5f) // Vector3.one es lo mismo que new Vector3(1, 1, 1)
            .setEaseOutBack()
            .setIgnoreTimeScale(true); // Útil si el menú se puede abrir en pausa

        isSettingsPopUpActive = true;
    }

    public void HideSettingsPopUp()
    {
        LeanTween.scale(SettingsPopUp, Vector3.zero, 0.5f) // Vector3.zero es lo mismo que new Vector3(0, 0, 0)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                SettingsPopUp.SetActive(false);
                backBlack.SetActive(false); // Oculta el fondo negro al terminar
            });

        isSettingsPopUpActive = false;
    }

    /// <summary>
    /// Cierra la aplicación o detiene la ejecución en el editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("El jugador ha pulsado el botón de salir.");

        
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
        

        #else
                Application.Quit();
        #endif
    }

    public void OpenUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("La URL proporcionada es nula o vacía.");
        }
    }

}
