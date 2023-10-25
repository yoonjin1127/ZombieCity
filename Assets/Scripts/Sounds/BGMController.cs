using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    GameObject BackgroundMusic;
    AudioSource backmusic;

    [SerializeField] private AudioSource titleAudio;
    [SerializeField] private AudioSource gameAudio;

    public UnityAction sceneChangeEvent;

    private void Awake()
    {
        BackgroundMusic = GameObject.Find("BGMController");
        backmusic = titleAudio;  // ������ ���� Ÿ��Ʋ ����� ���
        backmusic.Play();
        DontDestroyOnLoad(BackgroundMusic); // ������� ��� ���
        
        // ���� �� �̺�Ʈ ���
        SceneManager.sceneLoaded += SceneChangeEvent;
    }


    private void SceneChangeEvent(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "TitleScene" && backmusic == titleAudio)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "GameScene" && backmusic == gameAudio)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            backmusic.Stop();
            backmusic = gameAudio;  // ���Ӿ� ����� ����
            backmusic.Play();
        }

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            backmusic.Stop();
            backmusic = titleAudio;  // ���Ӿ� ����� ����
            backmusic.Play();
        }
    }


}
