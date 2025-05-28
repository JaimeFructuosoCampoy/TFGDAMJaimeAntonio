using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingInManager : MonoBehaviour
{
    public TMP_InputField NameInputField;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public void SigninClick()
    {
        string email = EmailInput.text;
        string password = PasswordInput.text;
        string username = NameInputField.text;
        SupabaseDao.Instance.SignUp(email, password, username);
    }
    // Start is called before the first frame update
    void Start()
    {

    }
}
