using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private Animator animator;

    public AudioSource dieAudio;     // 사망 소리

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행
        base.OnEnable();
        UpdateUI();
    }

    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity() 실행
        base.RestoreHealth(newHealth);
        // 체력 갱신
        UpdateUI();
    }

    private void UpdateUI()
    {
        // UIManager의 UpdateHealthText 함수에 체력을 인수로 넘겨 체력UI 갱신
        // 사망상태면 0을 넘기고 아니면 현재 체력을 넘김
        UIManager.Instance.UpdateHealthText(dead ? 0f : health);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        // 데미지를 입는데 실패했다면 false 반환
        if (!base.ApplyDamage(damageMessage)) return false;

        // 성공했다면
        // PlayHitEffect도 재생
        animator.SetTrigger("hit");
        UpdateUI();
        return true;
    }

    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();

        // 체력 슬라이더 비활성화
        UpdateUI();

        // 사망음 재생
        dieAudio.Play();

        // 사망 애니메이션 재생
        animator.SetTrigger("Die");
    }
}
