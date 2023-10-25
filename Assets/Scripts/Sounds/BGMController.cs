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
        backmusic = titleAudio;  // 시작할 때는 타이틀 오디오 재생
        backmusic.Play();
        DontDestroyOnLoad(BackgroundMusic); // 배경음악 계속 재생
        
        // 시작 시 이벤트 등록
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
            backmusic = gameAudio;  // 게임씬 오디오 변경
            backmusic.Play();
        }

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            backmusic.Stop();
            backmusic = titleAudio;  // 게임씬 오디오 변경
            backmusic.Play();
        }
    }


}
