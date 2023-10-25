using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Utility : MonoBehaviour
{
    // NavMesh 위에서 어떤 위치와 변경을 기준으로 랜덤한 위치 리턴
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask) // 인수 => 중심위치, 반경거리, 검색할 Area(내부적으로 int)
    {
        // 랜덤한 위치 randomPos 생성
        // 센터를 중점으로 하여 반지름 distance 내에 랜덤한 위치 리턴
        var randomPos = Random.insideUnitSphere * distance + center;    // center을 중점으로 하여 반지름 (반경) distance 내에 랜덤한 위치 리턴. *Random.insideUnitSphere*은 반지름 1짜리의 구 내에서 랜덤한 위치를 리턴해주는 프로퍼티다.

        // NavMesh인 areaMask이면서 distance 내에서 randomPos에 가장 가까운 위치 생성
        NavMeshHit hit;     // NavMesh 샘플링의 결과를 담을 컨테이너. Raycast hit과 비슷

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);     // areaMask에 해당하는 NavMesh 중에서 randomPos로부터 distance 반경 내에서 randomPos에 *가장 가까운* 위치를 하나 찾아서 그 결과를 hit에 담음.

        return hit.position;        // 샘플링 결과 위치인 hit.position 리턴
    }

    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
}
