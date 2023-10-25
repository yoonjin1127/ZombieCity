using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Shop : BaseUI
{
    public RectTransform uiGroup;
    public Animator anim; 
    public bool IsShop { get; set; }
    private UIManager uiManager;
    private SettingUI settingUI;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Button life;
    [SerializeField] private Button gun;
    [SerializeField] private Button grenade;
    [SerializeField] private GameObject curGun;
    [SerializeField] private GameObject changeGun;

    [SerializeField] private Throw throwInstance;

    private void OnEnable()
    {
        life.onClick.AddListener(OnLifeButtonClicked);
        gun.onClick.AddListener(OnGunButtonClicked);
        grenade.onClick.AddListener(OnGrenadeButtonClicked);
    }

    private void OnDisable()
    {
        life.onClick.RemoveListener(OnLifeButtonClicked);
        gun.onClick.RemoveListener(OnGunButtonClicked);
        grenade.onClick.RemoveListener(OnGrenadeButtonClicked);
    }

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
    }

    private void ShopIn()
    {
        settingUI = FindObjectOfType<SettingUI>();
        if (settingUI.IsSettingUI == true)
        {
            return;
        }
        else
        {
            Enter();
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            IsShop = true;
            playerInput.enabled = false;
        }
    }

    public void ShopOut()
    {
            Exit();
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            IsShop = false;
            playerInput.enabled = true;
    }

    private void OnShopIn(InputValue value)
    {
        if (IsShop)
            ShopOut();
        else
            ShopIn();
    }

    // ��ǰ�� �������� ��
    public void Purchase()
    {
        int curScore = 0;
        curScore = GameManager.Instance.score;
        Debug.Log(curScore);

    }

    public void OnLifeButtonClicked()
    {
        if (GameManager.Instance.score >= 400)
        {
            GameManager.Instance.AddScore(-400);
            UIManager.Instance.UpdateLifeText(PlayerMove.Instance.lifeRemains+1);
        }
        
    }

        public void OnGunButtonClicked()
    {
        if (GameManager.Instance.score >= 700)
        {
            GameManager.Instance.AddScore(-700);
            // ���� �� ��Ȱ��ȭ, ���濹���� �� Ȱ��ȭ
            curGun.active = false;
            changeGun.active = true;
            // �ߺ� ���Ű� �Ұ����ϵ��� ��ư�� ����
            gun.onClick.RemoveListener(OnGunButtonClicked);
        }
    }

        public void OnGrenadeButtonClicked()
    {
        if (GameManager.Instance.score >= 500)
        {
            GameManager.Instance.AddScore(-500);
            throwInstance.totalThrows+=1;
            Debug.Log(throwInstance.totalThrows);
        }
    }

}
