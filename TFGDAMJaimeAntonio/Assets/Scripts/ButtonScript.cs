using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public TMP_Text TitleText; 
    public AudioClip ButtonClickSound; //sonido al pulsar el botón
    public AudioSource AudioSource; //componente de audio para reproducir el sonido
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Metodo para cambiar de escena.
    /// </summary>
    /// <param name="scene"></param>
    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Metodos para la animacion de los botones al pasar el raton por encima.
    /// </summary>
    public void OnPointerEnterButton()
    {
        LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }
    public void OnPointerExitButton()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    /// <summary>
    /// Metodo para abrir el manual de usuario.
    /// </summary>
    public void OpenLinkUserManual()
    {
        Application.OpenURL("https://drive.google.com/file/d/1aFLvwhuT0TQh1ZecA3QP-gR9uF4NTa-X/view?usp=sharing");
    }
    
    /// <summary>
    /// Metodo para hafcer que suene el sonido al pulsar el botón.
    /// </summary>
    public void PlayButtonClickSound()
    {
        if (AudioSource != null && ButtonClickSound != null)
        {
            AudioSource.PlayOneShot(ButtonClickSound);
        }
    }

}
