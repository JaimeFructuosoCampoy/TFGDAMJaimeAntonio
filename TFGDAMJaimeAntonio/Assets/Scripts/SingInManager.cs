using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingInManager : MonoBehaviour
{
    public TMP_InputField NameInputField;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    public TMP_InputField PasswordReInput;


    public TMP_Text ErrorText;
    public void SigninClick()
    {
        string email = EmailInput.text;
        string password = PasswordInput.text;
        string username = NameInputField.text;
        string passwordRe = PasswordReInput.text;
        if (password.Equals(passwordRe))
        {
            SupabaseDao.Instance.SignUp(email, password, username, SignInError, SwitchToMenuScene);
        } 
        else
        {
            ShowError("Las contraseñas no coinciden. Por favor, intente de nuevo.");
        }
        
    }

    private void ShowError(string errorMessage)
    {
        ErrorText.text = errorMessage;
        ErrorText.gameObject.SetActive(true);
    }

    private void SignInError()
    {
        ShowError("Error al registrarse.");
    }

    private void SwitchToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    // Start is called before the first frame update
    void Start()
    {

    }
}
