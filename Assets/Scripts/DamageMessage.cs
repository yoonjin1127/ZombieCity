using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    // ������ ���� ������Ʈ
    public GameObject damager;
    // ���ݷ�
    public float amount;
    // ������ ���� ��ġ
    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
