using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // �̱���
    private static EffectManager m_instance;
    public static EffectManager Instance    // ����Ʈ �Ŵ��� �ν��Ͻ��� ���Ϲ��� ������Ƽ
    {
        get
        {
            // ����Ʈ �Ŵ����� �ν��Ͻ��� �ϳ��� �����ϵ��� ������
            if (m_instance == null) m_instance = FindObjectOfType<EffectManager>();
            return m_instance;
        }
    }

    // ��ƼŬ ����
    public enum EffectType
    {
        Common,     // ��κ� ���� �Ϲ� ��ź ����Ʈ
        Flesh       // �Ǻγ� �쿡 �Ѿ��� �ε������� ����� �ǰ� Ƣ�� ����Ʈ
    }

    public ParticleSystem commonHitEffectPrefab;    // ��Ż ������ �Ҵ�
    public ParticleSystem FleshHitEffectPrefab;     // �÷��� ������ �Ҵ�

    // �����̴� ������� ����Ʈ�� �����Ǿ��� ����
    // �� ����� ����Ʈ�� �θ�� �Ҵ����־�� �Ѵ�
    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null, EffectType effectType = EffectType.Common)
    {
        // common�� �⺻���� �����ϰ�, Flesh�� ��� �ٲ۴�
        var targetPrefab = commonHitEffectPrefab;

        if (effectType == EffectType.Flesh)
        {
            targetPrefab = FleshHitEffectPrefab;
        }

        // ����Ʈ ���� ����(from ������)
        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null) effect.transform.SetParent(parent);
        effect.Play();
    }


}
