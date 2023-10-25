using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingButton;

    public void LoadScene()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Setting()
    {

    }
}
