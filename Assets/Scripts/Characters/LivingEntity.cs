using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// ��� ����ü (�÷��̾�, ����) ������Ʈ���� �����ϴ� ��ɵ��� ������ Ŭ����

public class LivingEntity : MonoBehaviour, IHitable
{
    public float startingHealth = 100f;     // ��� ����ü���� ���� ����ü��
    public float health { get; protected set; } // ���� ü��
    public bool dead { get; protected set; } // ��� ����

    public event Action OnDeath;        // ActionŸ���� ���ó�� �̺�Ʈ

    private const float minTimeBetDamaged = 0.1f;   // ���ݰ� ���� ������ �ּ� ��� �ð�
    private float lastDamagedTime;      // ���������� ���ݴ��� ����
    
    // ���ݹ��� �� �ִ� ���������� ����
    // ���������� ���ݴ��� �ð�+�ּ� ��� �ð� ���� ���ݴ��� ��쿡�� �����Ѵ�
    // ª���ð� ������ �ߺ������� ���� ���ؼ�
    protected bool Isvulnerable
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }

    // ������ �� ����
    // ���� �Լ��� ������, �ڽ� Ŭ������ �������̵��Ѵٸ� ����, �׷��� �ʴٸ� �״�� ����
    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        // ���� ���� || �����ڰ� �� �ڽ� || �̹� ���� ���¶�� false �� ����
        if (Isvulnerable || damageMessage.damager == gameObject || dead) return false;

        // lastDamagedTime ���� �ð����� ������Ʈ, health ����, health 0���ϸ� Die ȣ��
        lastDamagedTime = Time.time;
        health -= damageMessage.amount;

        if (health <= 0) Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        // ���� ��� ���� x
        if (dead) return;
        // ü�� ȸ�� (���� ü�� = ���� ü�� + ȸ����)
        health += newHealth;
    }

    public virtual void Die()
    {
        // OnDeath �̺�Ʈ�� �ּ� �ϳ� �̻��� �Լ��� ��ϵǾ� �ִٸ� OnDeath() �Լ� ����
        if (OnDeath != null) OnDeath();
        // ����� ���·� ����
        dead = true;
    }

    public void Hit(RaycastHit hit, int damage)
    {
    }
}
