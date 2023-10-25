using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
    // ��ƼŬ �ý��� ������Ʈ Ǯ��
    public ParticleSystem commonShootParticle_Prefab;       // �ǰ� ��ƼŬ ����Ʈ ������
    public ParticleSystem fleshShootParticle_Prefab;
    public ParticleSystem muzzleParticle_Prefab;       // �߻� ����Ʈ ������
    public TrailRenderer trailParticle_Prefab;        // Ʈ���� ����Ʈ ������

    private ParticleSystem commonShootParticle;  // ��ƼŬ ����Ʈ
    private ParticleSystem fleshShootParticle;
    private ParticleSystem muzzleParticle;      // �߻� ����Ʈ
    private TrailRenderer trailParticle;       // Ʈ���� ����Ʈ

    public GameObject gunShootParticlePos;     // �ǰ� ��ƼŬ ����Ʈ ������ ��ġ(�ǰ���)
    public GameObject muzzleTrailParticlePos;       // �߻�, Ʈ���� ����Ʈ ������ �ѱ� ��ġ

    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)   // �̱���
        {
            instance = this;    // �Ҵ� �ȵ����� �Ҵ��Ŵ
        }    
        else if (instance !=this)
        {
            Destroy(this.gameObject);       // ���� �Ҵ��� �Ǿ��ִµ� �� ��ü�� �ƴ϶�� ����
        }

        // DontDestroyOnLoad(this.gameObject);      // �ٸ� ������ �Ѿ���� �� ������Ʈ�� ����
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

        Debug.Log("ShootParticleOn! �Լ� ���");
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
    public int score;  // ���� ���� ����
    public bool isGameOver { get; private set; } // ���� ���� ����

    // ������ �� �� �� ȣ��ǹǷ�, ����� �ÿ� ���� �۾��� ����� �Ѵ�
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
        // ���� ������ �ƴ� ���¿����� ���� ���� ����
        if (!isGameOver)
        {
            // ���� �߰�
            score += newScore;
            // ���� UI �ؽ�Ʈ ����
            UIManager.Instance.UpdateScoreText(score);
        }
    }

    // ���� ���� ó��
    public void EndGame()
    {
        // ���� ���� ���¸� ������ ����
        isGameOver = true;
        
        // ���� ���� UI�� Ȱ��ȭ
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
