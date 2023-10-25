using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// 모든 생명체 (플레이어, 몬스터) 오브젝트들이 공유하는 기능들을 정의한 클래스

public class LivingEntity : MonoBehaviour, IHitable
{
    public float startingHealth = 100f;     // 모든 생명체들의 공동 시작체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; } // 사망 여부

    public event Action OnDeath;        // Action타입의 사망처리 이벤트

    private const float minTimeBetDamaged = 0.1f;   // 공격과 공격 사이의 최소 대기 시간
    private float lastDamagedTime;      // 마지막으로 공격당한 시점
    
    // 공격받을 수 있는 상태인지의 여부
    // 마지막으로 공격당한 시간+최소 대기 시간 내에 공격당한 경우에는 무시한다
    // 짧은시간 사이의 중복공격을 막기 위해서
    protected bool Isvulnerable
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }

    // 시작할 때 리셋
    // 가상 함수로 설정해, 자식 클래스가 오버라이딩한다면 변경, 그렇지 않다면 그대로 간다
    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        // 무적 상태 || 가격자가 나 자신 || 이미 죽은 상태라면 false 후 종료
        if (Isvulnerable || damageMessage.damager == gameObject || dead) return false;

        // lastDamagedTime 현재 시간으로 업데이트, health 차감, health 0이하면 Die 호출
        lastDamagedTime = Time.time;
        health -= damageMessage.amount;

        if (health <= 0) Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        // 죽은 경우 실행 x
        if (dead) return;
        // 체력 회복 (현재 체력 = 현재 체력 + 회복량)
        health += newHealth;
    }

    public virtual void Die()
    {
        // OnDeath 이벤트에 최소 하나 이상의 함수가 등록되어 있다면 OnDeath() 함수 실행
        if (OnDeath != null) OnDeath();
        // 사망한 상태로 변경
        dead = true;
    }

    public void Hit(RaycastHit hit, int damage)
    {
    }
}
