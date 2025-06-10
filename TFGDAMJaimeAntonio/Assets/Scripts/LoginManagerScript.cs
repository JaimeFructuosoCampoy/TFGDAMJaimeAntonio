using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManagerScript : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    public TMP_Text ErrorText;
    public void LoginClick()
    {
        string email = EmailInput.text;
        string password = PasswordInput.text;
        SupabaseDao.Instance.Login(email, password, ShowError);
    }

    private void ShowError()
    {
        ErrorText.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
