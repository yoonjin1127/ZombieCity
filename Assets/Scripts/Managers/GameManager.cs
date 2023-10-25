using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
    // 파티클 시스템 오브젝트 풀링
    public ParticleSystem commonShootParticle_Prefab;       // 피격 파티클 이펙트 프리팹
    public ParticleSystem fleshShootParticle_Prefab;
    public ParticleSystem muzzleParticle_Prefab;       // 발사 이펙트 프리팹
    public TrailRenderer trailParticle_Prefab;        // 트레일 이펙트 프리팹

    private ParticleSystem commonShootParticle;  // 파티클 이펙트
    private ParticleSystem fleshShootParticle;
    private ParticleSystem muzzleParticle;      // 발사 이펙트
    private TrailRenderer trailParticle;       // 트레일 이펙트

    public GameObject gunShootParticlePos;     // 피격 파티클 이펙트 나오는 위치(피격자)
    public GameObject muzzleTrailParticlePos;       // 발사, 트레일 이펙트 나오는 총구 위치

    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)   // 싱글톤
        {
            instance = this;    // 할당 안됐으면 할당시킴
        }    
        else if (instance !=this)
        {
            Destroy(this.gameObject);       // 만약 할당이 되어있는데 이 객체가 아니라면 해제
        }

        // DontDestroyOnLoad(this.gameObject);      // 다른 씬으로 넘어가더라도 이 오브젝트는 유지
    }

    private void Start()
    {
        commonShootParticle = Instantiate(commonShootParticle_Prefab, gunShootParticlePos.transform.position, gunShootParticlePos.transform.rotation);
        fleshShootParticle = Instantiate(fleshShootParticle_Prefab, gunShootParticlePos.transform.position, gunShootParticlePos.transform.rotation);
        muzzleParticle = Instantiate(muzzleParticle_Prefab, muzzleTrailParticlePos.transform.position, muzzleTrailParticlePos.transform.rotation);
        trailParticle = Instantiate(trailParticle_Prefab, muzzleTrailParticlePos.transform.position, muzzleTrailParticlePos.transform.rotation);
    }

    private void Update()
    {
        
    }

    private void ShootParticleOn()
    {
        commonShootParticle.transform.position = gunShootParticlePos.transform.position;
        commonShootParticle.transform.rotation = gunShootParticlePos.transform.rotation;

        commonShootParticle.gameObject.SetActive(true);

        fleshShootParticle.transform.position = gunShootParticlePos.transform.position;
        fleshShootParticle.transform.rotation = gunShootParticlePos.transform.rotation;

        fleshShootParticle.gameObject.SetActive(true);

        muzzleParticle.transform.position = muzzleTrailParticlePos.transform.position;
        muzzleParticle.transform.rotation = muzzleTrailParticlePos.transform.rotation;

        muzzleParticle.gameObject.SetActive(true);

        trailParticle.transform.position = muzzleTrailParticlePos.transform.position;
        trailParticle.transform.rotation = muzzleTrailParticlePos.transform.rotation;

        trailParticle.gameObject.SetActive(true);

        Debug.Log("ShootParticleOn! 함수 사용");
    }
    */
    private static GameManager instance;
    private static PoolManager poolManager;
    private static ResourceManager resourceManager;
    private static SoundManager soundManager;
    private static TPSCamera tpsCamera;

    public static GameManager Instance { get { return instance; } }
    public static PoolManager Pool { get { return poolManager; } }
    public static ResourceManager Resource { get { return resourceManager; } }
    public static SoundManager Sound { get { return soundManager; } }

    public int Score { get { return score; } set { score = value; } }
    public int score;  // 현재 게임 점수
    public bool isGameOver { get; private set; } // 게임 오버 상태

    // 시작할 때 한 번 호출되므로, 재시작 시에 같은 작업을 해줘야 한다
    private void Awake()
    {
        isGameOver = false;

        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        InitManagers();
    }

    public void Restart()
    {
        // Awake();
        isGameOver = false;
    }

    public void AddScore(int newScore)
    {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameOver)
        {
            // 점수 추가
            score += newScore;
            // 점수 UI 텍스트 갱신
            UIManager.Instance.UpdateScoreText(score);
        }
    }

    // 게임 오버 처리
    public void EndGame()
    {
        // 게임 오버 상태를 참으로 변경
        isGameOver = true;
        
        // 게임 오버 UI를 활성화
        UIManager.Instance.SetActiveGameOverUI(true);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void InitManagers()
    {
        GameObject poolObj = new GameObject();
        poolObj.name = "PoolManager";
        poolObj.transform.parent = transform;
        poolManager = poolObj.AddComponent<PoolManager>();

        GameObject resourceObj = new GameObject();
        resourceObj.name = "ResourceManager";
        resourceObj.transform.parent = transform;
        resourceManager = resourceObj.AddComponent<ResourceManager>();

        GameObject soundObj = new GameObject();
        soundObj.name = "SoundManager";
        soundObj.transform.parent = transform;
        soundManager = soundObj.AddComponent<SoundManager>();
    }

}
