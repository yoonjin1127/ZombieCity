using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ���� ������Ʈ�� �ֱ������� ����
public class EnemySpawner : MonoBehaviour
{
    private readonly List<Enemy> enemies = new List<Enemy>();   // ������ ������ ��� ����Ʈ

    public float damageMax = 40f;       // �ִ� ���ݷ�
    public float damageMin = 20f;       // �ּ� ���ݷ�
    public Enemy enemyPrefab;           // ������ �� AI

    public float healthMax = 200f;      // �ִ� ü��
    public float healthMin = 100f;      // �ּ� ü��

    public Transform[] spawnPoints; // �� AI�� ��ȯ�� ��ġ��

    public float speedMax = 3f;        // �ִ� �ӵ�
    public float speedMin = 7f;       // �ּ� �ӵ�

    public Color strongEnemyColor = Color.red;  // ���� �� AI�� ������ �� �Ǻλ�(������)
    public int wave;        // ���� ���̺�

    private float spawnTime = 5;     // ���� �����ð�
    private bool isSpawning = false;    // ���� ������ ���θ� ��Ÿ���� ����

    Timer timer;

    private IEnumerator spawnCoroutine()
    {      
        timer = FindObjectOfType<Timer>();
        timer.enabled = true;
        timer.Restart();
        isSpawning = true;  // ���� ��
        yield return new WaitForSeconds(spawnTime);
        SpawnWave();
        isSpawning = false; // ���� ����
        
    }

    private void Update()
    {
        // ���ӿ��������� ���� �������� ����
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // ���� ���̶�� ������Ʈ ����
        if (isSpawning) return;

        // ���� ��� ����ģ ��� ���� ���� ����
        // ���� �缳�� ���
        if (enemies.Count <= 0)
        {
            // �ڷ�ƾ ����
            StartCoroutine(spawnCoroutine());
        }

        // UI ����
        UpdateUI();
    }

    // ���̺� ������ UI�� ǥ��
    private void UpdateUI()
    {
        // ���� ���̺�� ���� ���� �� ǥ��
        UIManager.Instance.UpdateWaveText(wave, enemies.Count);
    }

    // ���� ���̺꿡 ���� ���� ����
    private void SpawnWave()
    {
        // ���̺� 1����
        wave++;

        // ���� ���̺� + 1.5�� �ݿø� �� ������ŭ ���� ����
        var spawnCount = Mathf.RoundToInt(wave * 5f);

        // spawnCount��ŭ ���� ����
        for (var i = 0; i < spawnCount; i++)
        {
            // ���� ���⸦ 0%���� 100% ���̿��� ���� ����
            var enemyIntensity = Random.Range(0f, 1f);
            // �� ���� ó�� ����
            CreateEnemy(enemyIntensity);
        }
    }
    private void CreateEnemy(float intensity)
    {
        // intensity�� ������� ���� �ɷ�ġ ����
        var health = Mathf.Lerp(healthMin, healthMax, intensity);
        var damage = Mathf.Lerp(damageMin, damageMax, intensity);
        var speed = Mathf.Lerp(speedMin, speedMax, intensity);

        // intensity�� ������� �Ͼ���� enemyStrength ���̿��� ���� �Ǻλ� ����
        var skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);

        // ������ ��ġ�� �������� ����
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // �� ���������κ��� �� ����
        var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ������ ���� �ɷ�ġ�� ���� ��� ����
        enemy.SetUp(health, damage, speed, speed * 0.3f, skinColor);

        // ������ ���� ����Ʈ�� �߰�
        enemies.Add(enemy);

        // ���� OnDeath �̺�Ʈ�� �͸� �޼��� ���
        // ����� ���� ����Ʈ���� ����
        enemy.OnDeath += () => enemies.Remove(enemy);

        // ����� ���� 10�� �ڿ� �ı�
        enemy.OnDeath += () => Destroy(enemy.gameObject, 10f);

        // �� ��� �� ���� ���
        enemy.OnDeath += () => GameManager.Instance.AddScore(100);
    }

}
