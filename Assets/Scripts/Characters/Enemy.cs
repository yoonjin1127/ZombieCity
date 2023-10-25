using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   //  �׺���̼� ����� �����̴�
using UnityEditor.Playables;
using UnityEngine.ProBuilder.MeshOperations;

#if UNITY_EDITOR    // ��ó���� �ȿ� �־ �����ؾ� ��
using UnityEditor;
#endif

public class Enemy : LivingEntity
{
    [SerializeField] GameObject playerHitEffect;
    private enum State
    {
        Patrol,     // ���ƴٴϴ� ����
        Tracking,   // �÷��̾ �߰��ϴ� ����
        AttackBegin,    // ���� ����
        Attacking       // ����
    }

    private State state;    // ���� ����

    private NavMeshAgent agent; // ��ΰ�� AI ������Ʈ
    private Animator animator;

    public Transform attackRoot;    // ������ ��ġ
    public Transform eyeTransform;  // ������

    private AudioSource audioPlayer;    // ����� �ҽ� ������Ʈ, �Ҹ� �����
    public AudioClip hitClip;       // �ǰ� �� ����� �Ҹ�
    public AudioClip deathClip;     // ��� �� ����� �Ҹ�

    private Renderer skinRenderer;  // ������ ������Ʈ (�Ǻλ��� ���� ���̸� ��)

    public float runSpeed = 10f;    // ���� �̵� �ӵ�
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;  // ���� ���� ȸ�� ��� �����ð�. smoothDamp�� ����� ��.
    private float turnSmoothVelocity;  // smoothDamp�� ����� ��. �ε巴�� ȸ���ϴ� �ǽð� ��ȭ��

    public float damage = 30f;        // ���ݷ�
    public float attackRadius = 2f; // ���� �ݰ�(������)
    private float attackDistance;   // ���� �õ��Ÿ�

    public float fieldOfView = 50f; // ������ �þ� ��
    public float viewDistance = 10f;// ���� �� �� �ִ� �Ÿ�
    public float patrolSpeed = 3f;  // ���� ���ƴٴϴ� �ӵ�(Patrol ������ ��)

    private float idleTimer = 0f;
    public float idleTimeThreshold = 0.01f;    // �� �ð� �̻����� �����ִٸ� ��� ��Ž��

    [HideInInspector] public LivingEntity targetEntity; // ������ ���
    public LayerMask whatIsTarget;  // ���� ��� ���̾�

    private RaycastHit[] hits = new RaycastHit[10]; // 10������(�浹����)�� ����ĳ��Ʈ��Ʈ �迭. ��������� ���� ����.
    private List<LivingEntity>lastAttackedTargets = new List<LivingEntity>();   // ������ �Ȱ��� ��󿡰� �ι� ������� �ʵ��� �ϱ� ���� ����Ʈ

    // ������ ����� �����ϴ����� ����
    // ������ ����� ���� && ���� ���°� �ƴ�
    private bool hasTarget => targetEntity != null && !targetEntity.dead;

    [SerializeField] AudioSource zombieAudio;

    
    // �� �󿡼��� ���̴� �����
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() // ������Ʈ�� ���õ��� �� ����� ���̰� �ϴ� �̺�Ʈ �Լ�
    {
        if (attackRoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(attackRoot.position, attackRadius);   // ���ݹݰ��� ���� �׷��ش�
        }

        var leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);    // ������ �þ� ����(ȣ ���)
        var leftRayDirection = leftRayRotation * transform.forward; // ���� ���������� ���ϴ� ����
        Handles.color = Color.white;
        // ��ä���� �׸��� DrawSolidArc
        // ������ ��ġ���� �׷�����, ����3�� ���� ��������, ���������� fieldOfView ������ŭ ȸ���ϸ�, �������� viewDistance�� ��ä���� �׸�
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
    }
#endif
    

    private void Awake()
    {
        // ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        skinRenderer = GetComponentInChildren<Renderer>();  // ������ �ڽ� ������Ʈ�� �߿��� Renderer ������Ʈ�� ���� ������Ʈ�� Renderer Ÿ������ ��������

        // ������ �õ��ϴ� �Ÿ�
        // ������ �ڱ� �ڽ��� ��ġ�κ��� attackRoot������ �Ÿ��� ���ݹݰ��� ����
        attackDistance = Vector3.Distance(transform.position,
                            new Vector3(attackRoot.position.x, transform.position.y, attackRoot.position.z)) + 
                            attackRadius;

        attackDistance += agent.radius;

        // ���߰� ������ �õ��� �Ÿ�
        agent.stoppingDistance = attackDistance;

        // ���� �ӵ� �ʱ�ȭ
        agent.speed = patrolSpeed;
    }

    // ���� AI�� �ʱ� ������ �����ϴ� �¾� �޼���
    public void SetUp(float health, float damage,
        float runSpeed, float patrolSpeed, Color skinColor)
    {
        // ü�� ����
        this.startingHealth = health;   // �ʱ� ���� ü��
        this.health = health;           // ü��

        // ����޽� ������Ʈ�� �̵��ӵ� ����
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;           // ���ݷ�

        // �������� ������� ���׸����� �÷��� ����, ���� ���� ����
        skinRenderer.material.color = skinColor;

        agent.speed = patrolSpeed;      // ������ ����� patrolSpeed�� �ٽ� ����
    }

    private void Start()
    {
        // ���� ������Ʈ Ȱ��ȭ�� ���ÿ� AI�� ������ƾ ����
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (dead) return;

        // ���� ������ �Ÿ��� ������ ������ �������� �˻�
        if (state == State.Tracking &&
            Vector3.Distance(targetEntity.transform.position, transform.position) <= attackDistance)
        {
            BeginAttack();
        }

        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼��� ���
        // ��ҿ��� �Ȱ�, ������ ���� �޸��� ����
        animator.SetFloat("MoveSpeed", agent.desiredVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (dead) return;

        // ������ �����ϰų� �����ϰ� �ִ� �߿��� ���� �ڽ��� Ÿ���� ���� ȸ���ϵ��� ��
        if (state == State.AttackBegin || state == State.Attacking)
        {
            var lookRotation =
                // Ÿ�� ��ġ - �ڱ� �ڽ��� ��ġ = Ÿ���� �ٶ󺸴� ����
                Quaternion.LookRotation(targetEntity.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            // �ε巴�� ����
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                ref turnSmoothVelocity, turnSmoothTime);
        }

        if (state == State.Attacking)
        {
            zombieAudio.Play();
            var direction = transform.forward;
            // ���� ���� FixedUpdate��������� ���սð��� TIme.fixedDeltaTime���� �̵��� �Ÿ��� �ȴ�
            var deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            // OverlapSphere�� ����ϸ� ������ ���̿� ���������� �߻��� �� �ִ�
            // SphereCastNonAlloc �Լ��� ������ ���� ���̿� ��ġ�� �浹ü�� �˻�, �����Ѵ� 
            // size�� hit �迭�� ũ��
            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRadius, direction, hits, deltaDistance,
                whatIsTarget);

            for (var i = 0; i < size; i++)
            {
                // ������ �͵��� ���� hit �迭���� Player Character�� ���� ��ȸ�ϸ� ã�� ������ ó���Ѵ�
                var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>(); 

                // lastAttackedTargets ������ ����
                if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                {
                    var message = new DamageMessage();
                    message.amount = damage;        // ���ݷ�
                    message.damager = gameObject;   // �����ڴ� ���� �ڽ�

                    // ������ �� ����
                    if (hits[i].distance <=0f)
                    {
                        message.hitPoint = attackRoot.position;
                    }
                    else
                    {
                        message.hitPoint = hits[i].point;
                    }

                    // ������ ���� ����
                    message.hitNormal = hits[i].normal;

                    attackTargetEntity.ApplyDamage(message);

                    // �̹� ������ ���� �����̶�� �濡��
                    lastAttackedTargets.Add(attackTargetEntity);

                    break;          // ���� ����� ã������ for�� ����
                }
            }
        }
    }
    private IEnumerator UpdatePath()
    {
        // ����ִ� ���� ���ѷ���
        while (!dead)
        {
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    agent.speed = runSpeed;
                }

                // ���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
                agent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                if (targetEntity != null) targetEntity = null;

                // ���� ���°� �ƴϾ��ٸ� ���� �ٽ� �������·� ����
                if (state != State.Patrol)
                {
                    state = State.Patrol;
                    agent.speed = patrolSpeed;
                }

                // �ϴ� �þ߸� ���� �����ϱ� ���� NavMesh ���� � ������ �������� �̵��ϰ� �Ѵ�.
                // �Ÿ��� 1���� ���� ������ ��, ������ռ�-��������϶�
                if (agent.remainingDistance <= 1f || agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                }

                // 20 ������ �������� ���� ������ ���� �׷��� ��, ���� ��ġ�� ��� �浹ü�� ������
                // �þ� �� ���� �ִ� ��� Collider���� �迭�� ��´�
                // ��, whatIsTarget ���̾ ���� �ݶ��̴��� ���������� ���͸�
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                // ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� LivingEntity ã��
                foreach (var collider in colliders)
                {
                    if (!IsTargetOnSight(collider.transform)) continue;

                    var livingEntity = collider.GetComponent<LivingEntity>();

                    // LivingEntity ������Ʈ�� �����ϸ�, �ش� LivingEntity�� ����ִٸ�,
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // ���� ����� �ش� LivingEntity�� ����
                        targetEntity = livingEntity;

                        // for�� ���� ��� ����
                        break;
                    }
                }
                // AI �̵����� üũ
                if (agent.velocity.sqrMagnitude <= 0.01f)
                {
                    // ���� �ð� ���� ���
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleTimeThreshold)
                    {
                        // ��� ��Ž���� �̵�
                        var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                        agent.SetDestination(patrolPosition);

                        // Ž�� �� Ÿ�̸� �ʱ�ȭ
                        idleTimer = 0f;
                    }
                }

                else
                {
                    // AI�� �����̴� ���� ��� Ÿ�̸� �ʱ�ȭ
                    idleTimer = 0f;
                }
            }
            // 0.05�� �ð� ������ �θ鼭 ��� �ִ� ���� ���� ���� �ݺ� ó��
            yield return new WaitForSeconds(0.01f);
        }
    }
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false; // ������ ó��

        if (targetEntity == null)
        {
            targetEntity = damageMessage.damager.GetComponent<LivingEntity>();  
        }

        RaycastHit hit;

        // TODO : ������ƮǮ�� ��ġ��
        // EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal, transform, EffectManager.EffectType.Flesh);   // Ÿ�� ��ƼŬ ȿ�� ���
        audioPlayer.PlayOneShot(hitClip);   // ����� ���

        return true;
    }

    public void BeginAttack()
    {
        state = State.AttackBegin;

        agent.isStopped = true;     // ��� ���� ����
        animator.SetTrigger("Attack");  // ���� �ִϸ��̼� ����
    }

    public void EnableAttack()
    {
        state = State.Attacking;

        // �������� ������ ������ ����ִ� lastAttackeTargets ����Ʈ ���
        lastAttackedTargets.Clear();
    }

    public void DisableAttack()
    {
        if (hasTarget)
        {
            // Ÿ���� ������ �����ִٸ� ���¸� State.Tracking���� �ǵ���
            state = State.Tracking;
        }
        else
        {
            // Ÿ���� ���� ���ٸ� ���¸� State.Patrol�� �ǵ���
            state = State.Patrol;
        }
        // AI Agent�� �ٽ� �����̵��� agent.isStopped = false
        agent.isStopped = false;
    }

    private bool IsTargetOnSight(Transform target)
    {
        // ����ĳ��Ʈ ��� ������ ��� �����̳�
        RaycastHit hit;

        var direction = target.position - eyeTransform.position;

        direction.y = eyeTransform.forward.y;

        // �� ������ �þ߰��� ����� �ȵǸ�
        if (Vector3.Angle(direction, eyeTransform.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        direction = target.position - eyeTransform.position;

        // �þ߰� ���� �����ϴ��� ������ ��ֹ��� �ε�ġ�� �ʰ� ��ǥ�� �� ��ƾ� ��
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance, whatIsTarget))
        {
            if (hit.transform == target) return true;
            else return false;
        }

        return true;
    }

    public override void Die()
    {
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����
        base.Die();

        // �ٸ� AI���� �������� �ʵ��� �ڽ��� ��� �ݶ��̴����� ��Ȱ��ȭ
        GetComponent<Collider>().enabled = false;

        // AI ������ �����ϰ� ����޽� ������Ʈ�� ��Ȱ��ȭ
        agent.enabled = false;

        // ��� �ִϸ��̼� ���
        animator.applyRootMotion = true;
        animator.SetTrigger("Die");

        // ��� ȿ���� ���
        if (deathClip != null) audioPlayer.PlayOneShot(deathClip);
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        this.health -= 100;
        Vector3 reactVec = transform.position - explosionPos;
    }

}
