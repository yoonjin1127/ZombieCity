using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject zombieHitEffect;
    [SerializeField] GameObject structureHitEffect;
    [SerializeField] ParticleSystem muzzleEffect;
    [SerializeField] GameObject bulletTrail;
    [SerializeField] float bulletSpeed;
    [SerializeField] public float maxDistance;
    [SerializeField] int damage;

    // 총의 상태를 표현하는데 사용할 타입을 선언
    public enum State
    {
        Ready,      // 발사 준비
        Empty,      // 탄창이 빔
        Reloading   // 재장전 중
    }

    // 현재 총의 상태
    public State state { get; private set; }

    private PlayerShooter gunHolder;

    private Gun gun;

    // 발사 소리
    public AudioSource shotAudio;
    // 재장전 소리
    public AudioSource reloadAudio;
    // 남은 전체 탄약
    public int ammoRemain = 100;
    // 현재 탄창에 남아있는 탄약
    public int magAmmo;
    // 탄창 용량
    public int magCapacity;

    // 총알이 발사될 위치
    // public Transform fireTransform;

    // 총알 발사 간격
    public float timeBetFire = 0.12f;
    // 재장전 소요시간
    public float reloadTime = 1.8f;

    // 총을 마지막으로 발사한 시점
    private float lastFireTime;

    // 총알을 쏴서는 안되는 대상을 거르기 위한 레이어
    private LayerMask excludeTarget;

    public void SetUp(PlayerShooter gunHolder)
    {
        this.gunHolder = gunHolder;
        excludeTarget = gunHolder.excludeTarget;
    }

    private void OnEnable()
    {
        magAmmo = magCapacity;  // 현재 탄창을 가득 채우기
        state = State.Ready;    // 준비상태로 설정
        lastFireTime = 0;       // 마지막 발사시점 초기화
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /*
    public void OnCollisionEnter(Collision collision)
    {
        MeleeAttack();

        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            lastFireTime = Time.time;
            DamageMessage damageMessage = new DamageMessage();

            damageMessage.damager = gunHolder.gameObject;
            damageMessage.amount = damage;

            // 파티클 시스템
            // 좀비에게 적중하는 파티클 생성
            GameObject effect = GameManager.Pool.Get(zombieHitEffect);
            StartCoroutine(ReleaseRoutine(effect));

            IHitable target = collision.gameObject.GetComponent<IHitable>();
            target?.ApplyDamage(damageMessage);
        }
    }
    */

    public virtual void Fire()
    {
        gameObject.SetActive(true);
        RaycastHit hit;

        magAmmo--;
        if (magAmmo <= 0)
        {
            // 탄약이 없다면 현재 상태를 Empty로 갱신
            state = State.Empty;
            // 현재 탄창의 탄수가 0아래로 떨어지지 않도록 0으로 갱신
            magAmmo = 0;
        }

        // 총알이 맞은 곳을 저장
        var hitPosition = Vector3.zero;

        // 총알이 적중했다면
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
        {
            muzzleEffect.Play();
            // 인터페이스는 GetComponent에도 동작시킬 수 있다
            IHitable hitable = hit.transform.GetComponent<IHitable>();


            StartCoroutine(TrailRoutine(muzzleEffect.transform.position, hit.point));

            hitable?.Hit(hit, damage);
            /* if (hitable != null)
                 hitable.Hit(hit, damage); */

            lastFireTime = Time.time;   // 마지막 발사시점 갱신

            // 충돌한 상대방으로부터 IHitable 오브젝트 가져오기 시도
            var target
                = hit.collider.GetComponent<IHitable>();          

            // todo : 허공에 쐈을때는 출력 안되게끔 구현
            // IHitable을 가져오는데 성공했다면
            if (target != null)
            {
                shotAudio.Play();
                muzzleEffect.Play();
                DamageMessage damageMessage;

                damageMessage.damager = gunHolder.gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hit.point;
                damageMessage.hitNormal = hit.normal;

                // 파티클 시스템
                // 좀비에게 적중하는 파티클 생성
                GameObject effect = GameManager.Pool.Get(zombieHitEffect);
                effect.transform.position = hit.point;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.transform.parent = hit.transform;

                StartCoroutine(ReleaseRoutine(effect));

                // 상대방의 OnDamage 함수를 실행시켜서 데미지 주기
                target.ApplyDamage(damageMessage);
            }
            // IHitable을 가져오지 못했다면 (벽이나 구조물)
            else
            {
                shotAudio.Play();
                muzzleEffect.Play();
                // 파티클 시스템
                // 건물에 적중하는 파티클 생성
                GameObject effect = GameManager.Pool.Get(structureHitEffect);
                effect.transform.position = hit.point;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.transform.parent = hit.transform;

                StartCoroutine(ReleaseRoutine(effect));
            }
        }
        // 적중하지 않았다면
        else if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            muzzleEffect.Play();
            StartCoroutine(TrailRoutine(muzzleEffect.transform.position, Camera.main.transform.forward * maxDistance));
        }
    }

    IEnumerator ReleaseRoutine(GameObject effect)
    {
        yield return new WaitForSeconds(3f);
        GameManager.Pool.Release(effect); 
    }

    public bool Reload()
    {
        if (state == State.Reloading ||
            ammoRemain <= 0 || magAmmo >= magCapacity)
            // 이미 재장전 중이거나, 남은 총알이 없거나
            // 탄창에 총알이 이미 가득한 경우 재장전 할 수 없다
            return false;

        // 재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        // 재장전 소리 재생
        reloadAudio.Play();

        // 재장전 소요시간만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);

        // 탄창에 채울 탄약을 계산
        var ammoToFill = magCapacity - magAmmo;

        // 채워야할 탄약이 남은 탄약보다 더 많다면
        // 채워야할 탄약 수를 남은 탄약 수에 맞춰 줄인다
        if (ammoRemain < ammoToFill) ammoToFill = ammoRemain;

        // 탄창을 채운다
        magAmmo += ammoToFill;
        // 남은 탄약에서, 탄창에 채운만큼 탄약을 뺀다
        ammoRemain -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }

    IEnumerator TrailRoutine(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject trail = GameManager.Pool.Get(bulletTrail);
        trail.transform.position = startPoint;
        trail.transform.rotation = Quaternion.identity;
        // 두 갈래가 나오지 않게 초기화 진행
        trail.GetComponent<TrailRenderer>().Clear();

        float totalTime = Vector2.Distance(startPoint, endPoint) / bulletSpeed;

        float rate = 0;

        while (rate < 1)
        {
            trail.transform.position = Vector3.Lerp(startPoint, endPoint, rate);
            rate += Time.deltaTime / totalTime;

            yield return null;
        }
        // 트레일 반납
        GameManager.Pool.Release(trail);
    }
}
