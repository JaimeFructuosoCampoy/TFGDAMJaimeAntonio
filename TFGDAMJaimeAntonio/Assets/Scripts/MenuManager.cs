using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMP_Text TitleText;

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
}
