using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private Animator animator;

    public AudioSource dieAudio;     // ��� �Ҹ�

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // LivingEntity�� OnEnable() ����
        base.OnEnable();
        UpdateUI();
    }

    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity() ����
        base.RestoreHealth(newHealth);
        // ü�� ����
        UpdateUI();
    }

    private void UpdateUI()
    {
        // UIManager�� UpdateHealthText �Լ��� ü���� �μ��� �Ѱ� ü��UI ����
        // ������¸� 0�� �ѱ�� �ƴϸ� ���� ü���� �ѱ�
        UIManager.Instance.UpdateHealthText(dead ? 0f : health);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        // �������� �Դµ� �����ߴٸ� false ��ȯ
        if (!base.ApplyDamage(damageMessage)) return false;

        // �����ߴٸ�
        // PlayHitEffect�� ���
        animator.SetTrigger("hit");
        UpdateUI();
        return true;
    }

    public override void Die()
    {
        // LivingEntity�� Die() ����(��� ����)
        base.Die();

        // ü�� �����̴� ��Ȱ��ȭ
        UpdateUI();

        // ����� ���
        dieAudio.Play();

        // ��� �ִϸ��̼� ���
        animator.SetTrigger("Die");
    }
}
