using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityScene = UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // �̱������� ����
    private static UIManager instance;  // �̱���. private
    private SettingUI settingUI;
    private Shop shop;

    public static UIManager Instance    // �� ������Ƽ�� ���� �ν��Ͻ��� ���Ϲ���
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>(); // �ν��Ͻ��� �̹� ������ �� ���� ����
            return instance;
        }
    }

    // UHD Canvas�� ���� UI��ҵ�
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Text healthText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Text ammoText;
    [SerializeField] private TextMeshProUGUI waveText;


    public void UpdateAmmoText(int magAmmo, int remainAmmo)     // 'ammoText' ���� źâ UI ����
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    public void UpdateScoreText(int newScore)   // 'scoreText' ���� UI ����
    {
        scoreText.text = "Score :" + newScore;
    }

    public void UpdateWaveText(int waves, int count)    // 'waveText' ���� ���� �� UI ����
    {
        waveText.text = "Wave : " + waves + "/Enemy Left : " + count;
    }

    public void UpdateLifeText(int count)   // 'lifeText' ���� ���� �� UI ����
    {
        lifeText.text = "Life : " + count;
    }

    public void UpdateHealthText(float health)  // 'healthText' ���� HP UI ����
    {
        healthText.text = Mathf.Floor(health).ToString();    // ü���� �Ҽ����� ������ �� ���ڿ��� �ٲ�
    }

    public void SetActiveGameOverUI(bool active)    // ���� ������ 'GameOverUI' Ȱ��ȭ
    {
        gameoverUI.SetActive(active);
    }

    /*
    public void UpdateCrossHairPosition(Vector3 worldPosition) // �ش� ��ġ�� ũ�ν� ��� Ui ǥ��
    {
        crosshair.UpdatePosition(worldPosition);
    }
    */

    public void CancleButton()
    {
        settingUI = FindObjectOfType<SettingUI>();
        shop = FindObjectOfType<Shop>();

        if (settingUI.IsSettingUI == true)
        {
            settingUI.SettingUIOut();
        }
        else
        {
            shop.ShopOut();
        }
    }

    public void TitleButton()
    {
        SceneManager.LoadScene("TitleScene");
        Time.timeScale = 1;
    }


    public void SetActiveCrossHair(bool active)     // ũ�ν� ��� UI Ȱ��ȭ
    {
        crosshair.SetActive(active);
    }

    // Restart��ư�� OnClick �̺�Ʈ �Լ� ��Ͽ� �־��
    public void GameRestart()   // ���� Over ���¿��� Restart ��ư�� ������ �� �����ų �Լ�. ���� �����
    {    
        SceneManager.LoadScene("LoadScene"); // �ε���
        GameManager.Instance.Restart();
        GameManager.Instance.Score = 0;
        // UnityScene.LoadSceneMode.Single("LoadScene");
    }
}
