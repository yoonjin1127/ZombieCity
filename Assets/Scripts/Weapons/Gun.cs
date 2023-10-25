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

    // ���� ���¸� ǥ���ϴµ� ����� Ÿ���� ����
    public enum State
    {
        Ready,      // �߻� �غ�
        Empty,      // źâ�� ��
        Reloading   // ������ ��
    }

    // ���� ���� ����
    public State state { get; private set; }

    private PlayerShooter gunHolder;

    private Gun gun;

    // �߻� �Ҹ�
    public AudioSource shotAudio;
    // ������ �Ҹ�
    public AudioSource reloadAudio;
    // ���� ��ü ź��
    public int ammoRemain = 100;
    // ���� źâ�� �����ִ� ź��
    public int magAmmo;
    // źâ �뷮
    public int magCapacity;

    // �Ѿ��� �߻�� ��ġ
    // public Transform fireTransform;

    // �Ѿ� �߻� ����
    public float timeBetFire = 0.12f;
    // ������ �ҿ�ð�
    public float reloadTime = 1.8f;

    // ���� ���������� �߻��� ����
    private float lastFireTime;

    // �Ѿ��� ������ �ȵǴ� ����� �Ÿ��� ���� ���̾�
    private LayerMask excludeTarget;

    public void SetUp(PlayerShooter gunHolder)
    {
        this.gunHolder = gunHolder;
        excludeTarget = gunHolder.excludeTarget;
    }

    private void OnEnable()
    {
        magAmmo = magCapacity;  // ���� źâ�� ���� ä���
        state = State.Ready;    // �غ���·� ����
        lastFireTime = 0;       // ������ �߻���� �ʱ�ȭ
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

            // ��ƼŬ �ý���
            // ���񿡰� �����ϴ� ��ƼŬ ����
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
            // ź���� ���ٸ� ���� ���¸� Empty�� ����
            state = State.Empty;
            // ���� źâ�� ź���� 0�Ʒ��� �������� �ʵ��� 0���� ����
            magAmmo = 0;
        }

        // �Ѿ��� ���� ���� ����
        var hitPosition = Vector3.zero;

        // �Ѿ��� �����ߴٸ�
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
        {
            muzzleEffect.Play();
            // �������̽��� GetComponent���� ���۽�ų �� �ִ�
            IHitable hitable = hit.transform.GetComponent<IHitable>();


            StartCoroutine(TrailRoutine(muzzleEffect.transform.position, hit.point));

            hitable?.Hit(hit, damage);
            /* if (hitable != null)
                 hitable.Hit(hit, damage); */

            lastFireTime = Time.time;   // ������ �߻���� ����

            // �浹�� �������κ��� IHitable ������Ʈ �������� �õ�
            var target
                = hit.collider.GetComponent<IHitable>();          

            // todo : ����� �������� ��� �ȵǰԲ� ����
            // IHitable�� �������µ� �����ߴٸ�
            if (target != null)
            {
                shotAudio.Play();
                muzzleEffect.Play();
                DamageMessage damageMessage;

                damageMessage.damager = gunHolder.gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hit.point;
                damageMessage.hitNormal = hit.normal;

                // ��ƼŬ �ý���
                // ���񿡰� �����ϴ� ��ƼŬ ����
                GameObject effect = GameManager.Pool.Get(zombieHitEffect);
                effect.transform.position = hit.point;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.transform.parent = hit.transform;

                StartCoroutine(ReleaseRoutine(effect));

                // ������ OnDamage �Լ��� ������Ѽ� ������ �ֱ�
                target.ApplyDamage(damageMessage);
            }
            // IHitable�� �������� ���ߴٸ� (���̳� ������)
            else
            {
                shotAudio.Play();
                muzzleEffect.Play();
                // ��ƼŬ �ý���
                // �ǹ��� �����ϴ� ��ƼŬ ����
                GameObject effect = GameManager.Pool.Get(structureHitEffect);
                effect.transform.position = hit.point;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.transform.parent = hit.transform;

                StartCoroutine(ReleaseRoutine(effect));
            }
        }
        // �������� �ʾҴٸ�
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
            // �̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�
            // źâ�� �Ѿ��� �̹� ������ ��� ������ �� �� ����
            return false;

        // ������ ó�� ����
        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ
        state = State.Reloading;
        // ������ �Ҹ� ���
        reloadAudio.Play();

        // ������ �ҿ�ð���ŭ ó���� ����
        yield return new WaitForSeconds(reloadTime);

        // źâ�� ä�� ź���� ���
        var ammoToFill = magCapacity - magAmmo;

        // ä������ ź���� ���� ź�ຸ�� �� ���ٸ�
        // ä������ ź�� ���� ���� ź�� ���� ���� ���δ�
        if (ammoRemain < ammoToFill) ammoToFill = ammoRemain;

        // źâ�� ä���
        magAmmo += ammoToFill;
        // ���� ź�࿡��, źâ�� ä�ŭ ź���� ����
        ammoRemain -= ammoToFill;

        // ���� ���� ���¸� �߻� �غ�� ���·� ����
        state = State.Ready;
    }

    IEnumerator TrailRoutine(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject trail = GameManager.Pool.Get(bulletTrail);
        trail.transform.position = startPoint;
        trail.transform.rotation = Quaternion.identity;
        // �� ������ ������ �ʰ� �ʱ�ȭ ����
        trail.GetComponent<TrailRenderer>().Clear();

        float totalTime = Vector2.Distance(startPoint, endPoint) / bulletSpeed;

        float rate = 0;

        while (rate < 1)
        {
            trail.transform.position = Vector3.Lerp(startPoint, endPoint, rate);
            rate += Time.deltaTime / totalTime;

            yield return null;
        }
        // Ʈ���� �ݳ�
        GameManager.Pool.Release(trail);
    }
}
