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

    // ��ư�� ������ ������ ���ݰ� ��
    // ��ǲ�ý����� �̿�
    void Enter()
    {
        // ȭ�� ���߾ӿ� ������ ��
        uiGroup.anchoredPosition = Vector3.zero;
    }

    void Exit()
    {
        // ȭ�鿡�� ���������� ��
        uiGroup.anchoredPosition = Vector3.down * 1500;
        // ui �ʱ�ȭ
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
