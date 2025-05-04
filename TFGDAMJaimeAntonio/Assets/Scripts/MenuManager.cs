using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMP_Text TitleText;

    //PopUP Lenguage
    public GameObject PopUpLenguage;
    private bool isPopUpActive = false;

    // Start is called before the first frame update
    void Start()
    {
        AnimateTitle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AnimateTitle()
    {
        LeanTween.rotate(TitleText.gameObject, new Vector3(0f, 0f, 2.5f), 1f).setOnComplete(() =>
        {
            LeanTween.rotate(TitleText.gameObject, new Vector3(0f, 0f, -2.5f), 1f).setOnComplete(() =>
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
        LeanTween.scale(PopUpLenguage, new Vector3(2, 2, 1), 0.5f).setEaseOutBack();

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




}
