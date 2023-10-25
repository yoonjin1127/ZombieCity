using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class SettingUI : BaseUI
{
    public RectTransform uiGroup;
    //private bool IsSettingUI;
    public bool IsSettingUI { get; set; }
    private UIManager uiManager;
    private Shop shop;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject sliders;
    [SerializeField] private GameObject titleAndSetting;

    // 버튼을 누르고 상점을 여닫게 함
    // 인풋시스템을 이용
    void Enter()
    {
        // 화면 정중앙에 오도록 함
        uiGroup.anchoredPosition = Vector3.zero;
    }

    void Exit()
    {
        // 화면에서 내려가도록 함
        uiGroup.anchoredPosition = Vector3.down * 1500;
        // ui 초기화
        sliders.gameObject.SetActive(false);
        titleAndSetting.gameObject.SetActive(true);
    }

    public void SettingUIOut()
    {
        Exit();
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        IsSettingUI = false;
        playerInput.enabled = true;
    }

    private void SettingUIIn()
    {
        shop = FindObjectOfType<Shop>();
        if (shop.IsShop == true)
        {
            return;
        }

        else
        {
            Enter();
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            IsSettingUI = true;
            playerInput.enabled = false;
        }
    }

    private void OnSettingUIIn(InputValue value)
    {
        if (IsSettingUI)
            SettingUIOut();
        else
            SettingUIIn();
    }
}
