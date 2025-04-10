using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public TMP_Text TitleText;
    // Start is called before the first frame update
    void Start()
    {
        
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

}
