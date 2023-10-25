using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Utility : MonoBehaviour
{
    // NavMesh ������ � ��ġ�� ������ �������� ������ ��ġ ����
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask) // �μ� => �߽���ġ, �ݰ�Ÿ�, �˻��� Area(���������� int)
    {
        // ������ ��ġ randomPos ����
        // ���͸� �������� �Ͽ� ������ distance ���� ������ ��ġ ����
        var randomPos = Random.insideUnitSphere * distance + center;    // center�� �������� �Ͽ� ������ (�ݰ�) distance ���� ������ ��ġ ����. *Random.insideUnitSphere*�� ������ 1¥���� �� ������ ������ ��ġ�� �������ִ� ������Ƽ��.

        // NavMesh�� areaMask�̸鼭 distance ������ randomPos�� ���� ����� ��ġ ����
        NavMeshHit hit;     // NavMesh ���ø��� ����� ���� �����̳�. Raycast hit�� ���

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);     // areaMask�� �ش��ϴ� NavMesh �߿��� randomPos�κ��� distance �ݰ� ������ randomPos�� *���� �����* ��ġ�� �ϳ� ã�Ƽ� �� ����� hit�� ����.

        return hit.position;        // ���ø� ��� ��ġ�� hit.position ����
    }

    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
}
