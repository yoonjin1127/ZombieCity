using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // 싱글톤
    private static EffectManager m_instance;
    public static EffectManager Instance    // 이펙트 매니저 인스턴스를 리턴받을 프로퍼티
    {
        get
        {
            // 이펙트 매니저의 인스턴스가 하나만 존재하도록 관리함
            if (m_instance == null) m_instance = FindObjectOfType<EffectManager>();
            return m_instance;
        }
    }

    // 파티클 상태
    public enum EffectType
    {
        Common,     // 대부분 사용될 일반 피탄 이펙트
        Flesh       // 피부나 살에 총알이 부딪혔을때 사용할 피가 튀는 이펙트
    }

    public ParticleSystem commonHitEffectPrefab;    // 메탈 프리팹 할당
    public ParticleSystem FleshHitEffectPrefab;     // 플래쉬 프리팹 할당

    // 움직이는 대상으로 이펙트가 생성되었을 때는
    // 그 대상을 이펙트의 부모로 할당해주어야 한다
    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null, EffectType effectType = EffectType.Common)
    {
        // common을 기본으로 설정하고, Flesh인 경우 바꾼다
        var targetPrefab = commonHitEffectPrefab;

        if (effectType == EffectType.Flesh)
        {
            targetPrefab = FleshHitEffectPrefab;
        }

        // 이펙트 복제 생성(from 프리팹)
        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null) effect.transform.SetParent(parent);
        effect.Play();
    }


}
