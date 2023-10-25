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
    // 싱글톤으로 구현
    private static UIManager instance;  // 싱글톤. private
    private SettingUI settingUI;
    private Shop shop;

    public static UIManager Instance    // 이 프로퍼티를 통해 인스턴스를 리턴받음
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>(); // 인스턴스가 이미 존재할 땐 받지 않음
            return instance;
        }
    }

    // UHD Canvas의 구성 UI요소들
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Text healthText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Text ammoText;
    [SerializeField] private TextMeshProUGUI waveText;


    public void UpdateAmmoText(int magAmmo, int remainAmmo)     // 'ammoText' 남은 탄창 UI 갱신
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    public void UpdateScoreText(int newScore)   // 'scoreText' 점수 UI 갱신
    {
        scoreText.text = "Score :" + newScore;
    }

    public void UpdateWaveText(int waves, int count)    // 'waveText' 남은 적의 수 UI 갱신
    {
        waveText.text = "Wave : " + waves + "/Enemy Left : " + count;
    }

    public void UpdateLifeText(int count)   // 'lifeText' 남은 생명 수 UI 갱신
    {
        lifeText.text = "Life : " + count;
    }

    public void UpdateHealthText(float health)  // 'healthText' 남은 HP UI 갱신
    {
        healthText.text = Mathf.Floor(health).ToString();    // 체력의 소숫점을 내림한 후 문자열로 바꿈
    }

    public void SetActiveGameOverUI(bool active)    // 게임 오버시 'GameOverUI' 활성화
    {
        gameoverUI.SetActive(active);
    }

    /*
    public void UpdateCrossHairPosition(Vector3 worldPosition) // 해당 위치에 크로스 헤어 Ui 표시
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


    public void SetActiveCrossHair(bool active)     // 크로스 헤어 UI 활성화
    {
        crosshair.SetActive(active);
    }

    // Restart버튼의 OnClick 이벤트 함수 목록에 넣어둠
    public void GameRestart()   // 게임 Over 상태에서 Restart 버튼을 눌렀을 때 실행시킬 함수. 게임 재시작
    {    
        SceneManager.LoadScene("LoadScene"); // 로딩씬
        GameManager.Instance.Restart();
        GameManager.Instance.Score = 0;
        // UnityScene.LoadSceneMode.Single("LoadScene");
    }
}
