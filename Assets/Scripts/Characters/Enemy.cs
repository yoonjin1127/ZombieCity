using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   //  네비게이션 사용을 위함이다
using UnityEditor.Playables;
using UnityEngine.ProBuilder.MeshOperations;

#if UNITY_EDITOR    // 전처리기 안에 넣어서 선언해야 함
using UnityEditor;
#endif

public class Enemy : LivingEntity
{
    [SerializeField] GameObject playerHitEffect;
    private enum State
    {
        Patrol,     // 돌아다니는 상태
        Tracking,   // 플레이어를 추격하는 상태
        AttackBegin,    // 공격 시작
        Attacking       // 공격
    }

    private State state;    // 좀비 상태

    private NavMeshAgent agent; // 경로계산 AI 에이전트
    private Animator animator;

    public Transform attackRoot;    // 때리는 위치
    public Transform eyeTransform;  // 눈높이

    private AudioSource audioPlayer;    // 오디오 소스 컴포넌트, 소리 재생기
    public AudioClip hitClip;       // 피격 시 재생할 소리
    public AudioClip deathClip;     // 사망 시 재생할 소리

    private Renderer skinRenderer;  // 렌더러 컴포넌트 (피부색에 따라 차이를 둠)

    public float runSpeed = 10f;    // 좀비 이동 속도
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;  // 좀비 방향 회전 사용 지연시간. smoothDamp에 사용할 것.
    private float turnSmoothVelocity;  // smoothDamp에 사용할 것. 부드럽게 회전하는 실시간 변화량

    public float damage = 30f;        // 공격력
    public float attackRadius = 2f; // 공격 반경(반지름)
    private float attackDistance;   // 공격 시도거리

    public float fieldOfView = 50f; // 좀비의 시야 각
    public float viewDistance = 10f;// 좀비가 볼 수 있는 거리
    public float patrolSpeed = 3f;  // 좀비가 돌아다니는 속도(Patrol 상태일 때)

    private float idleTimer = 0f;
    public float idleTimeThreshold = 0.01f;    // 이 시간 이상으로 멈춰있다면 경로 재탐색

    [HideInInspector] public LivingEntity targetEntity; // 추적할 대상
    public LayerMask whatIsTarget;  // 추적 대상 레이어

    private RaycastHit[] hits = new RaycastHit[10]; // 10사이즈(충돌지점)의 레이캐스트히트 배열. 범위기반의 공격 구현.
    private List<LivingEntity>lastAttackedTargets = new List<LivingEntity>();   // 공격이 똑같은 대상에게 두번 적용되지 않도록 하기 위한 리스트

    // 추적할 대상이 존재하는지의 여부
    // 추적할 대상이 존재 && 죽은 상태가 아님
    private bool hasTarget => targetEntity != null && !targetEntity.dead;

    [SerializeField] AudioSource zombieAudio;

    
    // 씬 상에서만 보이는 기즈모
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() // 오브젝트가 선택됐을 때 기즈모가 보이게 하는 이벤트 함수
    {
        if (attackRoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(attackRoot.position, attackRadius);   // 공격반경을 구로 그려준다
        }

        var leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);    // 좀비의 시야 범위(호 모양)
        var leftRayDirection = leftRayRotation * transform.forward; // 왼쪽 끝지점으로 향하는 방향
        Handles.color = Color.white;
        // 부채꼴을 그리는 DrawSolidArc
        // 눈높이 위치에서 그려지고, 벡터3업 축을 기준으로, 오른쪽으로 fieldOfView 각도만큼 회전하며, 반지름이 viewDistance인 부채꼴을 그림
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
    }
#endif
    

    private void Awake()
    {
        // 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        skinRenderer = GetComponentInChildren<Renderer>();  // 좀비의 자식 오브젝트들 중에서 Renderer 컴포넌트를 가진 오브젝트를 Renderer 타입으로 가져오기

        // 공격을 시도하는 거리
        // 좀비인 자기 자신의 위치로부터 attackRoot사이의 거리에 공격반경을 더함
        attackDistance = Vector3.Distance(transform.position,
                            new Vector3(attackRoot.position.x, transform.position.y, attackRoot.position.z)) + 
                            attackRadius;

        attackDistance += agent.radius;

        // 멈추고 공격을 시도할 거리
        agent.stoppingDistance = attackDistance;

        // 순찰 속도 초기화
        agent.speed = patrolSpeed;
    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void SetUp(float health, float damage,
        float runSpeed, float patrolSpeed, Color skinColor)
    {
        // 체력 설정
        this.startingHealth = health;   // 초기 시작 체력
        this.health = health;           // 체력

        // 내비메쉬 에이전트의 이동속도 설정
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;           // 공격력

        // 렌더러가 사용중인 마테리얼의 컬러를 변경, 외형 색이 변함
        skinRenderer.material.color = skinColor;

        agent.speed = patrolSpeed;      // 위에서 변경된 patrolSpeed로 다시 적용
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (dead) return;

        // 추적 대상과의 거리를 따져서 공격을 실행할지 검사
        if (state == State.Tracking &&
            Vector3.Distance(targetEntity.transform.position, transform.position) <= attackDistance)
        {
            BeginAttack();
        }

        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        // 평소에는 걷고, 추적할 때는 달리게 설정
        animator.SetFloat("MoveSpeed", agent.desiredVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (dead) return;

        // 공격을 시작하거나 공격하고 있는 중에는 좀비 자신이 타겟을 향해 회전하도록 함
        if (state == State.AttackBegin || state == State.Attacking)
        {
            var lookRotation =
                // 타겟 위치 - 자기 자신의 위치 = 타겟을 바라보는 방향
                Quaternion.LookRotation(targetEntity.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            // 부드럽게 설정
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                ref turnSmoothVelocity, turnSmoothTime);
        }

        if (state == State.Attacking)
        {
            zombieAudio.Play();
            var direction = transform.forward;
            // 좀비가 다음 FixedUpdate실행까지의 사잇시간인 TIme.fixedDeltaTime동안 이동한 거리가 된다
            var deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            // OverlapSphere을 사용하면 프레임 사이에 오차범위가 발생할 수 있다
            // SphereCastNonAlloc 함수는 움직인 궤적 사이에 겹치는 충돌체를 검사, 감지한다 
            // size는 hit 배열의 크기
            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRadius, direction, hits, deltaDistance,
                whatIsTarget);

            for (var i = 0; i < size; i++)
            {
                // 감지된 것들이 모인 hit 배열에서 Player Character인 것을 순회하며 찾아 공격을 처리한다
                var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>(); 

                // lastAttackedTargets 안잡힘 수정
                if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                {
                    var message = new DamageMessage();
                    message.amount = damage;        // 공격량
                    message.damager = gameObject;   // 공격자는 좀비 자신

                    // 공격이 들어간 지점
                    if (hits[i].distance <=0f)
                    {
                        message.hitPoint = attackRoot.position;
                    }
                    else
                    {
                        message.hitPoint = hits[i].point;
                    }

                    // 공격이 들어가는 방향
                    message.hitNormal = hits[i].normal;

                    attackTargetEntity.ApplyDamage(message);

                    // 이미 공격을 가한 상대방이라는 뜻에서
                    lastAttackedTargets.Add(attackTargetEntity);

                    break;          // 공격 대상을 찾았으니 for문 종료
                }
            }
        }
    }
    private IEnumerator UpdatePath()
    {
        // 살아있는 동안 무한루프
        while (!dead)
        {
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    agent.speed = runSpeed;
                }

                // 추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
                agent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                if (targetEntity != null) targetEntity = null;

                // 정찰 상태가 아니었다면 이제 다시 정찰상태로 변경
                if (state != State.Patrol)
                {
                    state = State.Patrol;
                    agent.speed = patrolSpeed;
                }

                // 일단 시야를 통해 감지하기 전에 NavMesh 위의 어떤 임의의 지점으로 이동하게 한다.
                // 거리가 1보다 적게 남았을 때, 경로적합성-막힌경로일때
                if (agent.remainingDistance <= 1f || agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                }

                // 20 유닛의 반지름을 가진 가상의 구를 그렸을 때, 구와 겹치는 모든 충돌체를 가져옴
                // 시야 각 내에 있는 모든 Collider들을 배열에 담는다
                // 단, whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                // 모든 콜라이더들을 순회하면서, 살아있는 LivingEntity 찾기
                foreach (var collider in colliders)
                {
                    if (!IsTargetOnSight(collider.transform)) continue;

                    var livingEntity = collider.GetComponent<LivingEntity>();

                    // LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면,
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // 추적 대상을 해당 LivingEntity로 설정
                        targetEntity = livingEntity;

                        // for문 루프 즉시 정지
                        break;
                    }
                }
                // AI 이동여부 체크
                if (agent.velocity.sqrMagnitude <= 0.01f)
                {
                    // 일정 시간 멈춘 경우
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleTimeThreshold)
                    {
                        // 경로 재탐색후 이동
                        var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                        agent.SetDestination(patrolPosition);

                        // 탐색 후 타이머 초기화
                        idleTimer = 0f;
                    }
                }

                else
                {
                    // AI가 움직이는 중인 경우 타이머 초기화
                    idleTimer = 0f;
                }
            }
            // 0.05초 시간 간격을 두면서 살아 있는 동안 무한 루프 반복 처리
            yield return new WaitForSeconds(0.01f);
        }
    }
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false; // 데미지 처리

        if (targetEntity == null)
        {
            targetEntity = damageMessage.damager.GetComponent<LivingEntity>();  
        }

        RaycastHit hit;

        // TODO : 오브젝트풀로 고치기
        // EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal, transform, EffectManager.EffectType.Flesh);   // 타격 파티클 효과 재생
        audioPlayer.PlayOneShot(hitClip);   // 오디오 재생

        return true;
    }

    public void BeginAttack()
    {
        state = State.AttackBegin;

        agent.isStopped = true;     // 잠시 추적 정지
        animator.SetTrigger("Attack");  // 공격 애니메이션 시작
    }

    public void EnableAttack()
    {
        state = State.Attacking;

        // 이전까지 공격한 대상들이 담겨있는 lastAttackeTargets 리스트 비움
        lastAttackedTargets.Clear();
    }

    public void DisableAttack()
    {
        if (hasTarget)
        {
            // 타겟이 여전히 남아있다면 상태를 State.Tracking으로 되돌림
            state = State.Tracking;
        }
        else
        {
            // 타겟이 이제 없다면 상태를 State.Patrol로 되돌림
            state = State.Patrol;
        }
        // AI Agent가 다시 움직이도록 agent.isStopped = false
        agent.isStopped = false;
    }

    private bool IsTargetOnSight(Transform target)
    {
        // 레이캐스트 결과 정보가 담길 컨테이너
        RaycastHit hit;

        var direction = target.position - eyeTransform.position;

        direction.y = eyeTransform.forward.y;

        // 그 광선이 시야각을 벗어나선 안되며
        if (Vector3.Angle(direction, eyeTransform.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        direction = target.position - eyeTransform.position;

        // 시야각 내에 존재하더라도 광선이 장애물에 부딪치지 않고 목표에 잘 닿아야 함
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance, whatIsTarget))
        {
            if (hit.transform == target) return true;
            else return false;
        }

        return true;
    }

    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        GetComponent<Collider>().enabled = false;

        // AI 추적을 중지하고 내비메쉬 컴포넌트를 비활성화
        agent.enabled = false;

        // 사망 애니메이션 재생
        animator.applyRootMotion = true;
        animator.SetTrigger("Die");

        // 사망 효과음 재생
        if (deathClip != null) audioPlayer.PlayOneShot(deathClip);
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        this.health -= 200;
        Vector3 reactVec = transform.position - explosionPos;

        if (health <= 0)
        {
            Die();
            GameManager.Instance.AddScore(200);
        }
    }

}
