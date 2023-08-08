using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UsernameInput : MonoBehaviour
{
    [SerializeField]private TMP_InputField nameInputField;
    [SerializeField]private Button continueButton;
    [SerializeField]private Scene hub;

    private void Update()
    {
        InputField();
    }
    private void InputField()
    {
        if(nameInputField.text.Length >= 5f)
        {
            continueButton.interactable = true;
        }else
            continueButton.interactable = false;
    }
    public void ContinueClicked()
    {
        SceneManager.LoadScene("Hub");
    }
}
