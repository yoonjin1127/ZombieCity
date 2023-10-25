using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    public float health = 50;   // ü���� ȸ���� ��ġ

    public void Use(GameObject target)
    {
        // ���޹��� ���ӿ�����Ʈ�κ��� LivingEntity ������Ʈ �������� �õ�
        var life = target.GetComponent<LivingEntity>();

        // LivingEntity ������Ʈ�� �ִٸ�
        if (life != null)
        // ü��ȸ�� ����
        {
            life.RestoreHealth(health);
        }

        // ���Ǿ����Ƿ�, �ڽ��� �ı�
        Destroy(gameObject);
    }
}
