using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    [SerializeField] TPSCamera tps;
    [SerializeField] Slider senSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    public AudioMixer mix;

    private void Start()
    {
        // 초기화 작업
        BGMSlider.value = PlayerPrefs.GetFloat("BGMParam", 0.75f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXParam", 0.75f);
    }

    public void ChangedCameraSensitivity()
    {
        tps.cameraSensitivity = senSlider.value;
    }

    public void SetBGMVolume()
    {
        float BGMVal = BGMSlider.value;
        mix.SetFloat("BGMParam", Mathf.Log10(BGMVal) * 20);
        PlayerPrefs.SetFloat("BGMParam", BGMVal);
    }

    public void SetSFXVolume()
    {
        float SFXVal = SFXSlider.value;
        mix.SetFloat("SFXParam", Mathf.Log10(SFXVal) * 20);
        PlayerPrefs.SetFloat("SFXParam", SFXVal);
    }

}
