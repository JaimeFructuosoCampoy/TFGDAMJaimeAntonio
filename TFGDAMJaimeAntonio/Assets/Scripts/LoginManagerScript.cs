using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManagerScript : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public void LoginClick()
    {
        string email = EmailInput.text;
        string password = PasswordInput.text;
        SupabaseDAO.Instance.Login(email, password);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
