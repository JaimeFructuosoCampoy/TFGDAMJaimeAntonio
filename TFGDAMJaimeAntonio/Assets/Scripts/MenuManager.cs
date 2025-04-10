using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMP_Text Title;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AnimateTitle()
    {
        LeanTween.rotate(Title.gameObject, new Vector3(0f, 0f, 2.5f), 1f).setOnComplete(() =>
        {
            LeanTween.rotate(Title.gameObject, new Vector3(0f, 0f, -2.5f), 1f).setOnComplete(() =>
            {
                AnimateTitle();
            });
        });
    }

}
