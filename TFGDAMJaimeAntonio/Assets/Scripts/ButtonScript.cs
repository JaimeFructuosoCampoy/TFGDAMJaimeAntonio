using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public TMP_Text TitleText;
    public AudioClip ButtonClickSound;
    public AudioSource AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnPointerEnterButton()
    {
        LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnPointerExitButton()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void OpenLinkUserManual()
    {
        Application.OpenURL("https://drive.google.com/file/d/1aFLvwhuT0TQh1ZecA3QP-gR9uF4NTa-X/view?usp=sharing");
    }
    
    public void PlayButtonClickSound()
    {
        if (AudioSource != null && ButtonClickSound != null)
        {
            AudioSource.PlayOneShot(ButtonClickSound);
        }
    }

}
