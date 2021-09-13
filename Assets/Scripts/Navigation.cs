using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Voidless;
using Flamingo;



public class Navigation : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            int buttonIndex = i;
            Debug.Log("Button initialized " + i);
        }
    }

    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
        
    }
    public void Button0(int value)
    {
        Debug.Log("Button " + value + " Pressed");
    }
}
