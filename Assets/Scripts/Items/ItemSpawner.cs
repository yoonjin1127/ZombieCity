using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ����޽� ���� �ڵ�

// �ֱ������� �������� �÷��̾� ��ó�� �����ϴ� ��ũ��Ʈ
public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;  // ������ �����۵�
    public Transform playerTransform;   // �÷��̾��� Ʈ������

    private float lastSpawnTime;    // ������ ���� ����
    public float maxDistance = 5f;  // �÷��̾� ��ġ�κ��� �������� ��ġ�� �ִ� �ݰ�

    private float timeBetSpawn;     // ���� ����

    public float timeBetSpawnMax = 15f;  // �ִ� �ð� ����
    public float timeBetSpawnMin = 10f;  // �ּ� �ð� ����

    private void Start()
    {
        // ���� ���ݰ� ������ ���� ���� �ʱ�ȭ
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    // �ֱ������� ������ ���� ó�� ����
    private void Update()
    {
        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTransform != null)
        {
            lastSpawnTime = Time.time;  // ������ ���� �ð� ����
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);  // ���� �ֱ⸦ �������� ����
            Spawn();    // ���� ������ ����
        }
    }

    private void Spawn()
    {
        // �÷��̾� ��ó�� �׺� �޽� ���� ���� ��ġ�� �����´�
        var spawnPosition = Utility.GetRandomPointOnNavMesh(playerTransform.position, maxDistance, NavMesh.AllAreas);
        spawnPosition += Vector3.up * 0.5f; // �ٴڿ��� 0.5��ŭ ���� �ø��ϴ�.

        // ������ �� �ϳ��� �������� ��� ���� ��ġ�� �����Ѵ�
        // TODO : ������ƮǮ �Ẹ��
        var item = Instantiate(items[Random.Range(0, items.Length)], spawnPosition, Quaternion.identity);

        // ������ �������� 5�� �ڿ� �ı�
        Destroy(item, 5f);
    }
}
