using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public GameObject PopUpShop;
    private bool isPopUpActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Metodo que hace de interruptor para la confirmación de compra
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

    private void OpenPopUp()
    {
        PopUpShop.SetActive(true);

        PopUpShop.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(PopUpShop, new Vector3(2, 2, 1), 0.5f).setEaseOutBack();

        isPopUpActive = true;
    }

    /// <summary>
    /// Metodo para cerrar el popup de seleccion de idioma
    /// </summary>

    private void ClosePopUp()
    {
        LeanTween.scale(PopUpShop, new Vector3(0, 0, 0), 0.5f).setEaseInBack().setOnComplete(() =>
        {
            PopUpShop.SetActive(false);
        });

        isPopUpActive = false;
    }

    public void CancelBuy()
    {
        //Cerrar el popup de compra
        ClosePopUp();
    }

    public void ConfirmBuy()
    {
        //Cerrar el popup de compra
        ClosePopUp();

        //Logica para realizar compra
    }
}
