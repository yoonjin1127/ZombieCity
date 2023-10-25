using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    // 공격을 가한 오브젝트
    public GameObject damager;
    // 공격량
    public float amount;
    // 공격을 가한 위치
    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
